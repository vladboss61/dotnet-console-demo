using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                .WriteTo.File("logs/app_logs.txt", rollingInterval: RollingInterval.Hour)
                .CreateLogger();

            Log.Logger = logger;

            try
            {
                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((_, services) => 
                        new Startup(configuration, logger, args).ConfigureServices(services))
                    .Build();

                var application = host.Services.GetService<IApplication>();
                return application is null ? -1 : await application.LaunchAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return -1;
            }
        }
    }
}