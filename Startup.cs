
using CodingSeb.ExpressionEvaluator;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sample.console.Models.Arguments;
using sample.console.Services;
using sample.console.Services.Tasks;
using Serilog;

namespace sample.console;

internal sealed class Startup
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly string[] _args;

    public Startup(IConfiguration configuration, ILogger logger, string[] args)
    {
        _configuration = configuration;
        _logger = logger;
        _args = args;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var parserResult = Parser.Default.ParseArguments<CalculateOptions, StatisticsOptions>(_args);
        services.AddSingleton(_logger);
        services.AddSingleton(_configuration);
        services.AddSingleton<IConsoleOutput, ConsoleOutput>();

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
                _logger.Error("Something wrong with params, see --help");
            });
    }
}
