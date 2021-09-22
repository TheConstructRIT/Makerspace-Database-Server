using System;
using System.Collections.Generic;
using Construct.Core.Configuration;
using Microsoft.Extensions.Logging;
using Nexus.Logging;
using Nexus.Logging.Attribute;
using Nexus.Logging.Output;

namespace Construct.Core.Logging
{
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Logger to use for messages.
        /// </summary>
        private readonly Logger _logger;
        
        /// <summary>
        /// Creates the console logger.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        public ConsoleLogger(string name)
        {
            // Set up the console output.
            var consoleLogger = new ConsoleOutput()
            {
                IncludeDate = true,
                MinimumLevel = ConstructConfiguration.Configuration.Logging.ConsoleLevel,
                AdditionalLogInfo = new List<string>() { Logging.Log.Identifer, name },
                NamespaceWhitelist = new List<string>() { "Construct." },
            };
            
            // Set up the logging.
            this._logger = new Logger()
            {
                Outputs = new List<IOutput>() { consoleLogger }
            };
        }
        
        /// <summary>
        /// Begins the scope of the logger.
        /// </summary>
        /// <param name="state">State of what is being logged</param>
        /// <typeparam name="TState">Type of the state.</typeparam>
        /// <returns>Disposable object for the state.</returns>
        public IDisposable BeginScope<TState>(TState state) => default;

        /// <summary>
        /// Returns if the logger is enabled for the given log level.
        /// </summary>
        /// <param name="logLevel">Log level to check.</param>
        /// <returns>Whether the logger is enabled for the given log level.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= ConstructConfiguration.Configuration.Logging.ConsoleLevel;
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="logLevel">Log level of the message.</param>
        /// <param name="eventId">Event id of the log.</param>
        /// <param name="state">State of what is being logged.</param>
        /// <param name="exception">Exception of the log.</param>
        /// <param name="formatter">Formatter for the log.</param>
        /// <typeparam name="TState">Disposable object for the state.</typeparam>
        [LogTraceIgnore]
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Return if the log level isn't enabled.
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            // Log the message.
            this._logger.Log($"{formatter(state, exception)}", logLevel);
        }
    }
    
    public class LogProvider : ILoggerProvider
    {
        /// <summary>
        /// Creates a logger for the given category.
        /// </summary>
        /// <param name="categoryName">Category to create the logger for.</param>
        /// <returns>The logger to use.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger(categoryName);
        }

        /// <summary>
        /// Disposes of the log provider.
        /// </summary>
        public void Dispose()
        {
            
        }
    }
}