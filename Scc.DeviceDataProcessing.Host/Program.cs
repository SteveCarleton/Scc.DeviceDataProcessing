﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using NLog.Extensions.Logging;
using Scc.DeviceDataProcessing.DataModels;
using Scc.Services;


//#nullable enable
namespace Scc.DeviceDataProcessing.Hosting;

internal class ConsoleHost
{
    private static readonly ILogger log = Scc.Services.Logging.GetLogger(nameof(ConsoleHost));

    public static async Task<int> Main(string[] args)
    {
        try
        {
            Console.Title = "Device Data Processing";
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            ServiceCollection services = new();
            ConfigureServices(services);

            string? inputFile1 = config.GetValue<string>("inputJsonFile1");
            string? inputFile2 = config.GetValue<string>("inputJsonFile2");
            string? outputFile = config.GetValue<string>("outputJsonFile");

            log.LogDebug("The value for inputFile1 is: " + inputFile1);
            log.LogDebug("The value for inputFile2 is: " + inputFile2);
            log.LogDebug("The value for outputFile is: " + outputFile);

            Command mergeCommand = new("merge")
            {
                new Option<string>(
                    aliases: new string[] { "-if1", "--inputfile1-option" },
                    getDefaultValue: () => inputFile1 ?? string.Empty,
                    description:  "Json device data input file 1"),
                new Option<string>(
                    aliases: new string[] { "-if2", "--inputfile2-option" },
                    getDefaultValue: () => inputFile2 ?? string.Empty,
                    description: "Json device data input file 2"),
                 new Option<string>(
                     aliases: new string[] { "-of", "--outputfile-option" },
                     getDefaultValue: () => outputFile ?? string.Empty,
                     description: "Merged json device output data file")
            };

            Partner? partner = default!;
            Customer? customer = default!;

            mergeCommand.Handler = CommandHandler.Create<string, string, string>((inputFile1Option, inputFile2Option, outputFileOption) =>
            {
                log.LogInformation("The value for --input-file1 is: " + inputFile1Option);
                log.LogTrace("The value for --input-file2 is: " + inputFile2Option);
                log.LogDebug("The value for --output-file is: " + outputFileOption);

                JsonSerializerOptions options = new();
                options.Converters.Add((JsonConverter)new JsonDateTimeConverter());

                partner = JsonSerializer.Deserialize<Partner>(File.ReadAllText(inputFile1Option), options);
                customer = JsonSerializer.Deserialize<Customer>(File.ReadAllText(inputFile2Option), options);

                List<SensorResult> sensorResultList = new List<SensorResult>();

                foreach (Tracker tracker in partner!.Trackers)
                {
                    foreach (Sensor sensor in tracker.Sensors)
                    {
                        SensorResult sensorResult = new()
                        {
                            CompanyId = partner.PartnerId,
                            CompanyName = partner.PartnerName,
                            DeviceId = sensor.Id,
                            DeviceName = sensor.Name,

                            //FirstReadingDtm = new DateTime?(((IEnumerable<Crumb>)sensor.Crumbs).Select<Crumb, DateTime>((Func<Crumb, DateTime>)(c => c.CreatedDtm)).Min<DateTime>()),
                            FirstReadingDtm = (from c in sensor.Crumbs select c).Min(c => c.CreatedDtm),

                            //LastReadingDtm = new DateTime?(((IEnumerable<Crumb>)sensor.Crumbs).Select<Crumb, DateTime>((Func<Crumb, DateTime>)(c => c.CreatedDtm)).Max<DateTime>()),
                            LastReadingDtm = (from c in sensor.Crumbs select c).Max(c => c.CreatedDtm),

                            //TemperatureCount = new int?(source1.Count<Crumb[]>()),
                            TemperatureCount = (from s in tracker.Sensors where s.Name == "Tempurature" select s.Crumbs).Count<Crumb[]>(),

                            //IEnumerable<Crumb[]> source1 = ((IEnumerable<Sensor>)tracker.Sensors).Where<Sensor>((Func<Sensor, bool>)(s => s.Name == "Tempurature")).Select<Sensor, Crumb[]>((Func<Sensor, Crumb[]>)(s => s.Crumbs));
                            //Crumb[] source3 = source1.FirstOrDefault<Crumb[]>(),
                            //AverageTemperature = source3 != null ? new double?(((IEnumerable<Crumb>)source3).ToList<Crumb>().Average<Crumb>((Func<Crumb, double>)(c => c.Value))) : new double?(),
                            AverageTemperature = (from s in tracker.Sensors where s.Name == "Tempurature" select s.Crumbs)
                                .FirstOrDefault<Crumb[]>()?
                                .ToList()
                                .Average(c => c.Value),

                            //HumidityCount = new int?(source2.Count<Crumb[]>()),
                            HumidityCount = (from s in tracker.Sensors where s.Name == "Humidity" select s.Crumbs).Count<Crumb[]>(),

                            //IEnumerable<Crumb[]> source2 = ((IEnumerable<Sensor>)tracker.Sensors).Where<Sensor>((Func<Sensor, bool>)(s => s.Name == "Humidity")).Select<Sensor, Crumb[]>((Func<Sensor, Crumb[]>)(s => s.Crumbs));
                            //Crumb[] source4 = source2.FirstOrDefault<Crumb[]>(),
                            //AverageHumdity = source4 != null ? new double?(((IEnumerable<Crumb>)source4).ToList<Crumb>().Average<Crumb>((Func<Crumb, double>)(c => c.Value))) : new double?(),
                            AverageHumdity = (from s in tracker.Sensors where s.Name == "Humidity" select s.Crumbs)
                                .FirstOrDefault<Crumb[]>()?
                                .ToList()
                                .Average(c => c.Value),
                        };

                        sensorResultList.Add(sensorResult);
                    }
                }

                foreach (Device device in customer!.Devices)
                {
                    SensorResult sensorResult = new()
                    {
                        CompanyId = customer.CompanyId,
                        CompanyName = customer.Company,
                        DeviceId = device.DeviceId,
                        DeviceName = device.Name,

                        //sensorResult3.FirstReadingDtm = new DateTime?(((IEnumerable<SensorData>)device.SensorData).Select<SensorData, DateTime>((Func<SensorData, DateTime>)(c => c.DateTime)).Min<DateTime>());
                        FirstReadingDtm = (from s in device.SensorData select s).Min(c => c.DateTime),

                        //sensorResult3.LastReadingDtm = new DateTime?(((IEnumerable<SensorData>)device.SensorData).Select<SensorData, DateTime>((Func<SensorData, DateTime>)(c => c.DateTime)).Max<DateTime>());
                        LastReadingDtm = (from s in device.SensorData select s).Max(c => c.DateTime),

                        //sensorResult3.TemperatureCount = new int?(((IEnumerable<SensorData>)device.SensorData).Where<SensorData>((Func<SensorData, bool>)(d => d.SensorType == "TEMP")).Count<SensorData>());
                        TemperatureCount = (from s in device.SensorData where s.SensorType == "TEMP" select s).Count(),

                        //IEnumerable<SensorData> source5 = ((IEnumerable<SensorData>)device.SensorData).Where<SensorData>((Func<SensorData, bool>)(d => d.SensorType == "TEMP"));
                        //AverageTemperature = source5 != null ? new double?(source5.ToList<SensorData>().Average<SensorData>((Func<SensorData, double>)(d => d.Value))) : new double?();
                        AverageTemperature = (from s in device.SensorData where s.SensorType == "TEMP" select s).Average(d => d.Value),

                        //sensorResult3.HumidityCount = new int?(((IEnumerable<SensorData>)device.SensorData).Where<SensorData>((Func<SensorData, bool>)(d => d.SensorType == "HUM")).Count<SensorData>());
                        HumidityCount = (from s in device.SensorData where s.SensorType == "HUM" select s).Count(),

                        //IEnumerable<SensorData> source6 = ((IEnumerable<SensorData>)device.SensorData).Where<SensorData>((Func<SensorData, bool>)(d => d.SensorType == "HUM"));
                        //AverageHumdity = source6 != null ? new double?(source6.ToList<SensorData>().Average<SensorData>((Func<SensorData, double>)(d => d.Value))) : new double?();
                        AverageHumdity = (from s in device.SensorData where s.SensorType == "HUM" select s).Average(d => d.Value)
                    };

                    sensorResultList.Add(sensorResult);
                }
            });

            var rootCommand = new RootCommand { mergeCommand };
            rootCommand.Description = "DeviceDataProcessing";
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            log.LogError(log.ParseError(ex));
            return -1;
        }
    }

    static void ConfigureServices(ServiceCollection services) => services.AddLogging((Action<ILoggingBuilder>)(builder =>
    {
        builder.SetMinimumLevel(LogLevel.Trace);
        builder.AddNLog("nlog.config");
    }));

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs _)
    {
        Console.WriteLine("Notified of a thread exception... application is terminating.");
    }
}

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "MM-dd-yyyy HH:mm:ss";

    public override DateTime Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, DateFormat, null);
    }

    public override void Write(
      Utf8JsonWriter writer,
      DateTime value,
      JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}