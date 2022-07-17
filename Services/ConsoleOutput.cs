using System;
using Serilog;

namespace sample.console.Services
{
    /// <summary>
    /// Class to abstract console output including mirroring to ILogger
    /// </summary>
    public class ConsoleOutput: IConsoleOutput
    {
        private readonly ILogger _logger;

        public ConsoleOutput(ILogger logger)
        {
            _logger = logger;
        }
        
        public void WriteLine(string message)
        {
            _logger.Information("{Message}", message);
        }

        public void WriteError(string message)
        {
            _logger.Information("{Message}", message);
        }
    }
}