using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System;
using Scc.Services;
using Scc.DeviceDataProcessing.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace Scc.DeviceDataProcessing.Hosting;

class ConsoleHost
{
    static readonly Microsoft.Extensions.Logging.ILogger log = Logging.GetLogger(nameof(ConsoleHost));

    public static async Task<int> Main(string[] args)
    {
        try
        {
            Console.Title = "Device Data Processing";
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            ServiceCollection services = new();
            ConfigureServices(services);

            string? inputFile1 = config.GetValue<string>("inputJsonFile1");
            string? inputFile2 = config.GetValue<string>("inputJsonFile2");
            string? outputFile = config.GetValue<string>("outputJsonFile");

            log.LogDebug($"The value for inputFile1 is: {inputFile1}");
            log.LogDebug($"The value for inputFile2 is: {inputFile2}");
            log.LogDebug($"The value for outputFile is: {outputFile}");

            Command mergeCommand = new("merge")
            {
                new Option<string>(
                    aliases: new string[] { "-if1", "--inputfile1-option" },
                    getDefaultValue: () => inputFile1 ?? string.Empty,
                    description: "Json device data input file 1"),

                new Option<string>(
                    aliases: new string[] { "-if2", "--inputfile2-option" },
                    getDefaultValue: () => inputFile2 ?? string.Empty,
                    description: "Json device data input file 2"),

                new Option<string>(
                    aliases: new string[] { "-of", "--outputfile-option" },
                    getDefaultValue: () => outputFile ?? string.Empty,
                    description: "Merged json device output data file"),
            };

            Partner partner = default!;
            Customer customer = default!;

            mergeCommand.Handler = CommandHandler.Create<string, string, string>((inputFile1Option, inputFile2Option, outputFileOption) =>
            {
                log.LogInformation($"The value for --input-file1 is: {inputFile1Option}");
                log.LogTrace($"The value for --input-file2 is: {inputFile2Option}");
                log.LogDebug($"The value for --output-file is: {outputFileOption}");

                var options = new JsonSerializerOptions();
                options.Converters.Add(new JsonDateTimeConverter());

                string fileName = inputFile1Option;
                string jsonString = File.ReadAllText(fileName);
                partner = JsonSerializer.Deserialize<Partner>(jsonString, options)!;
                //log.LogDebug(partner.ToString());

                fileName = inputFile2Option;
                jsonString = File.ReadAllText(fileName);
                customer = JsonSerializer.Deserialize<Customer>(jsonString, options)!;

                List<SensorResult> results = new();

                foreach (Tracker tracker in partner.Trackers)
                {
                    foreach (Sensor sensor in tracker.Sensors)
                    {
                        //List<Crumb> crumbList = new()
                        //{
                        //    new Crumb
                        //    {
                        //        CreatedDtm = DateTime.Now,
                        //        Value = 1
                        //    },
                        //    new Crumb
                        //    {
                        //        CreatedDtm = DateTime.Now,
                        //        Value = 2
                        //    }
                        //};
                        //double average = crumbList.Average(x => x.Value);

                        //var crumbs = (from s in tracker.Sensors where s.Name == "Tempurature" select s.Crumbs).FirstOrDefault();
                        //List<Crumb> cList = crumbs.ToList();
                        //double avg = cList.Average(c => c.Value);

                        //List<Crumb> crumbs2 = (from s in tracker.Sensors where s.Name == "Tempurature" select s.Crumbs).FirstOrDefault()?.ToList<Crumb>();
                        //double avg2 = crumbs2.Average(c => c.Value);

                        //double? avg3 = (from s in tracker.Sensors where s.Name == "Tempurature" select s.Crumbs).FirstOrDefault()?.ToList().Average(c => c.Value);

                        var tempCrumbs = (from s in tracker.Sensors where s.Name == "Tempurature" select s.Crumbs);
                        var humidCrumbs = (from s in tracker.Sensors where s.Name == "Humidity" select s.Crumbs);

                        SensorResult sensorResult = new()
                        {
                            CompanyId = partner.PartnerId,
                            CompanyName = partner.PartnerName,
                            DeviceId = sensor.Id,
                            DeviceName = sensor.Name,
                            FirstReadingDtm = (from c in sensor.Crumbs select c.CreatedDtm).Min(),
                            LastReadingDtm = (from c in sensor.Crumbs select c.CreatedDtm).Max(),
                            //TemperatureCount = (from s in tracker.Sensors where s.Name == "Tempurature" select s.Crumbs).Count(),
                            TemperatureCount = tempCrumbs.Count(),
                            //AverageTemperature = (from s in tracker.Sensors 
                            //                      where s.Name == "Tempurature" 
                            //                      select s.Crumbs)
                            //                      .FirstOrDefault()?
                            //                      .ToList()
                            //                      .Average(c => c.Value)
                            AverageTemperature = tempCrumbs.FirstOrDefault()?.ToList().Average(c => c.Value),
                            HumidityCount = humidCrumbs.Count(),
                            AverageHumdity =  humidCrumbs.FirstOrDefault()?.ToList().Average(c => c.Value),
                        };

                        results.Add(sensorResult);
                    }
                }

                foreach (Device device in customer.Devices)
                {
                    SensorResult sensorResult = new()
                    {
                        CompanyId = customer.CompanyId,
                        CompanyName = customer.Company,
                        DeviceId = device.DeviceId,
                        DeviceName = device.Name,
                        FirstReadingDtm = (from c in device.SensorData select c.DateTime).Min(),
                        LastReadingDtm = (from c in device.SensorData select c.DateTime).Max(),
                        TemperatureCount = (from d in device.SensorData where d.SensorType == "TEMP" select d).Count(),
                        AverageTemperature = (from d in device.SensorData where d.SensorType == "TEMP" select d)?.ToList().Average(d => d.Value),
                        HumidityCount = (from d in device.SensorData where d.SensorType == "HUM" select d).Count(),
                        AverageHumdity = (from d in device.SensorData where d.SensorType == "HUM" select d)?.ToList().Average(d => d.Value),
                    };

                    results.Add(sensorResult);
                }

                //var services = new ServiceCollection();
                //ConfigureServices(services);

                //using (ServiceProvider serviceProvider = services.BuildServiceProvider())
                //{
                //    Application app = serviceProvider.GetService<Application>();
                //    //app.Read();
                //}
            });

            var rootCommand = new RootCommand { mergeCommand };
            rootCommand.Description = "DeviceDataProcessing";

            // Parse the incoming args and invoke the handler.
            return await rootCommand.InvokeAsync(args);
            //return 0;
        }
        catch (Exception exc)
        {
            log.LogError(log.ParseError(exc));
            return -1;
        }
    }

    static void ConfigureServices(ServiceCollection services)
    {
        //services.AddTransient<Application>();

        //services.AddScoped<Core.Finances>();

        //services.AddScoped<SamsCreditCardStatements>();
        //services.AddScoped<ChaseCreditCardStatements>();
        //services.AddScoped<ZealCreditUnionStatements>();
        //services.AddScoped<CscuStatements>();

        //services.AddScoped<List<SamsCreditCardStatement>>();
        //services.AddScoped<List<ChaseCreditCardStatement>>();
        //services.AddScoped<List<ZealCreditUnionStatement>>();
        //services.AddScoped<List<CscuStatement>>();

        //services.AddTransient<SamsCreditCardStatement>();
        //services.AddTransient<ChaseCreditCardStatement>();
        //services.AddTransient<ZealCreditUnionStatement>();
        //services.AddTransient<CscuStatement>();

        //services.AddSingleton<Categories>();
        //services.AddSingleton<Vendors>();
        //services.AddSingleton<Employers>();
        //services.AddSingleton<Checks>();
        //services.AddSingleton<Transactions>();

        //services.AddScoped<CategoryRepository>();

        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.AddNLog("nlog.config");
        });

        //IConfiguration config = new ConfigurationBuilder()
        //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //    .Build();

        //services.AddSingleton(config);

        //services.AddDbContext<FinancesDbContext>(options =>
        //    options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
        //        b => b.MigrationsAssembly(typeof(FinancesDbContext).Assembly.FullName)));
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs _)
    {
        Console.WriteLine("Notified of a thread exception... application is terminating.");
    }
}

//public class DateTimeConverter : JsonConverter<DateTime>
//{
//    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        Debug.Assert(typeToConvert == typeof(DateTime));
//        return DateTime.Parse(reader.GetString());
//    }

//    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
//    {
//        writer.WriteStringValue(value.ToUniversalTime().ToString("MM'-'dd'-'yyyy' 'HH':'mm':'ssZ"));
//    }
//}


public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    // Define the date format the data is in
    private const string DateFormat = "MM-dd-yyyy HH:mm:ss";

    // This is the deserializer
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, DateFormat, null);
    }

    // This is the serializer
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(
            DateFormat, CultureInfo.InvariantCulture));
    }
}
