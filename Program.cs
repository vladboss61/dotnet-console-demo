using System;
using System.IO;
using System.Threading.Tasks;
using CodingSeb.ExpressionEvaluator;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sample.console.Models.Arguments;
using sample.console.Services;
using sample.console.Services.Tasks;
using Serilog;

namespace sample.console
{
    internal class Program
    {
        /// <summary>
        /// Main routine
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Exit code</returns>
        public static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<ILogger>(logger);
                        services.AddSingleton<IConsoleOutput, ConsoleOutput>();

                        var parserResult = Parser.Default.ParseArguments<CalculateOptions, StatisticsOptions>(args);

                        parserResult
                            .WithParsed<CalculateOptions>(options =>
                            {
                                services.AddSingleton<ExpressionEvaluator>();
                                services.AddSingleton(options);
                                services.AddSingleton<IApplication, Calculate>();
                            })
                            .WithParsed<StatisticsOptions>(options =>
                            {
                                services.AddSingleton(options);
                                services.AddSingleton<IApplication, Statistics>();
                            })
                            .WithNotParsed(x =>
                            {
                                logger.Error("Something wrong with params, see --help");
                            });
                    })
                    .Build();

                // If a task was set up to run (i.e. valid command line params) then run it
                // and return the results
                var task = host.Services.GetService<IApplication>();
                return task == null
                    ? -1 // This can happen on --help or invalid arguments
                    : await task.Launch();
            }
            catch (Exception ex)
            {
                // Note that this should only occur if something went wrong with building Host
                logger.Error(ex.Message);
                return -1;
            }
        }
    }
}