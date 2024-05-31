using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;
using CodingSeb.ExpressionEvaluator;
using sample.console.Models.Arguments;
using sample.console.Models.Output;
using Serilog;

namespace sample.console.Services.Tasks
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class Calculate : IApplication
    {
        private readonly ExpressionEvaluator _evaluator;
        private readonly IConsoleOutput _console;
        private readonly ILogger _logger;
        private readonly CalculateOptions _options;

        public Calculate(
            ExpressionEvaluator evaluator,
            IConsoleOutput console,
            ILogger logger,
            CalculateOptions options)
        {
            _evaluator = evaluator;
            _console = console;
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// Outputs a feed to the specified file and format  
        /// </summary>
        public Task<int> LaunchAsync() => Task.Run(() =>
        {
            try
            {
                var expression = string.Join(" ", _options.Expression);
                _logger.Information("Evaluating {Evaluation}", expression);
                
                var start = DateTime.Now;
                var results = _evaluator.Evaluate(expression);
                var end = DateTime.Now;
                var output = CalculateOutput.FromResults(results);

                _console.WriteLine(_options.Format == OutputFormat.Text
                    ? output.ToString()
                    : output.ToJson());

                _logger.Information("Evaluated in {Elapsed} milliseconds",
                    end.Subtract(start).TotalMilliseconds);

                return 0;
            }
            catch (Exception ex)
            {
                _console.WriteError($"Unable to calculate: {ex.Message}");
                return -1;
            }
        });
    }
}