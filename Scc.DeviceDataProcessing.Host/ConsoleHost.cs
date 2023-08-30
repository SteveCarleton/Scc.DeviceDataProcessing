using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using NLog.Extensions.Logging;
using Scc.DeviceDataProcessing.Core;
using Scc.Services.Logging;

namespace Scc.DeviceDataProcessing.Hosting;

internal class ConsoleHost
{
    private static readonly ILogger log = Scc.Services.Logging.Logging.GetLogger(nameof(ConsoleHost));

    public static async Task<int> Main(string[] args)
    {
        const string title = "Device Data Processing";

        try
        {
            Console.Title = title;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            log.LogInformation($"Starting {title}: {DateTime.Now}");

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

            mergeCommand.Handler = CommandHandler.Create<string, string, string>((inputFile1Option, inputFile2Option, outputFileOption) =>
            {
                log.LogInformation("The value for --input-file1 is: " + inputFile1Option);
                log.LogInformation("The value for --input-file2 is: " + inputFile2Option);
                log.LogInformation("The value for --output-file is: " + outputFileOption);

                using (ServiceProvider serviceProvider = services.BuildServiceProvider())
                {
                    Application? app = serviceProvider.GetService<Application>();
                    app?.Merge(inputFile1Option, inputFile2Option, outputFileOption);
                }

                log.LogInformation($"Ending {title}: {DateTime.Now}");
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

    static void ConfigureServices(ServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog("nlog.config");
        });

        services.AddTransient<Application>();
        services.AddScoped<JsonProcessing>();
        services.AddScoped<DataProcessing>();
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs _)
    {
        Console.WriteLine("Notified of a thread exception... application is terminating.");
    }
}
