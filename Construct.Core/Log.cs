using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nexus.Logging;
using Nexus.Logging.Output;

namespace Construct.Core
{
    public class Log
    {
        /// <summary>
        /// Logger used for logging messages.
        /// </summary>
        private static Logger _logger;

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        /// <param name="identifier">Identifier to use with the logger.</param>
        public static void Initialize(string identifier)
        {
            // Set up the console output.
            var consoleLogger = new ConsoleOutput()
            {
                IncludeDate = true,
                MinimumLevel = LogLevel.Information,
                AdditionalLogInfo = new List<string>() { identifier },
                NamespaceWhitelist = new List<string>() { "Construct." },
            };
            
            // Set up the logging.
            _logger = new Logger()
            {
                Outputs = new List<IOutput>() { consoleLogger }
            };
        }

        /// <summary>
        /// Sets the minimum log level of the output.
        /// </summary>
        /// <param name="level">Minimum log level to use.</param>
        public static void SetMinimumLogLevel(LogLevel level)
        {
            foreach (var output in _logger.Outputs)
            {
                if (output is not QueuedOutput queuedOutput) continue;
                queuedOutput.MinimumLevel = level;
            }
        }
        
        /// <summary>
        /// Outputs a Trace message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        public static void Trace(object entry) => _logger.Log(entry, LogLevel.Trace);

        /// <summary>
        /// Outputs a Debug message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        public static void Debug(object entry) => _logger.Log(entry, LogLevel.Debug);

        /// <summary>
        /// Outputs an Info message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        public static void Info(object entry) => _logger.Log(entry, LogLevel.Information);

        /// <summary>
        /// Outputs a Warn message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        public static void Warn(object entry) => _logger.Log(entry, LogLevel.Warning);

        /// <summary>
        /// Outputs a Error message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        public static void Error(object entry) => _logger.Log(entry, LogLevel.Error);
    }
}