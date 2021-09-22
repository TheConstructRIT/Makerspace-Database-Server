using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nexus.Logging;
using Nexus.Logging.Attribute;
using Nexus.Logging.Output;

namespace Construct.Core.Logging
{
    public class Log
    {
        /// <summary>
        /// Logger used for logging messages.
        /// </summary>
        public static Logger Logger { get; set; }
        
        /// <summary>
        /// Identifier used by the logger.
        /// </summary>
        public static string Identifer { get; private set; }

        /// <summary>
        /// Minimum log level for the console output.
        /// </summary>
        private static LogLevel _minimumConsoleLogLevel = LogLevel.Information;

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        /// <param name="identifier">Identifier to use with the logger.</param>
        public static void Initialize(string identifier)
        {
            if (Logger != null) return;
            
            // Set up the console output.
            Identifer = identifier;
            var consoleLogger = new ConsoleOutput()
            {
                IncludeDate = true,
                MinimumLevel = _minimumConsoleLogLevel,
                AdditionalLogInfo = new List<string>() { identifier },
                NamespaceWhitelist = new List<string>() { "Construct." },
            };
            
            // Set up the logging.
            Logger = new Logger()
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
            _minimumConsoleLogLevel = level;
            
            // Set the log levels of existing loggers.
            if (Logger == null) return;
            foreach (var output in Logger.Outputs)
            {
                if (output is not QueuedOutput queuedOutput) continue;
                queuedOutput.MinimumLevel = level;
            }
        }

        /// <summary>
        /// Outputs a message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        /// <param name="level">Log level to output with.</param>
        public static void LogMessage(object entry, LogLevel level)
        {
            Logger?.Log(entry, level);
        }
        
        /// <summary>
        /// Outputs a Trace message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        [LogTraceIgnore]
        public static void Trace(object entry) => LogMessage(entry, LogLevel.Trace);

        /// <summary>
        /// Outputs a Debug message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        [LogTraceIgnore]
        public static void Debug(object entry) => LogMessage(entry, LogLevel.Debug);

        /// <summary>
        /// Outputs an Info message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        [LogTraceIgnore]
        public static void Info(object entry) => LogMessage(entry, LogLevel.Information);

        /// <summary>
        /// Outputs a Warn message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        [LogTraceIgnore]
        public static void Warn(object entry) => LogMessage(entry, LogLevel.Warning);

        /// <summary>
        /// Outputs a Error message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        [LogTraceIgnore]
        public static void Error(object entry) => LogMessage(entry, LogLevel.Error);

        /// <summary>
        /// Outputs a Critical message.
        /// </summary>
        /// <param name="entry">Entry to output.</param>
        [LogTraceIgnore]
        public static void Critical(object entry) => LogMessage(entry, LogLevel.Critical);
    }
}