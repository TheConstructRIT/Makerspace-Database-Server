using System.Collections.Generic;
using Construct.Core.Logging;
using Microsoft.Extensions.Logging;
using Nexus.Logging.Output;
using NUnit.Framework;

namespace Construct.Core.Test.Logging
{
    public class TestOutput : IOutput
    {
        /// <summary>
        /// Whether to include the date with the logs.
        /// </summary>
        public bool IncludeDate { get; set; }
        
        /// <summary>
        /// Additional information to show in the log entry.
        /// </summary>
        public List<string> AdditionalLogInfo { get; set; }

        /// <summary>
        /// Namespace whitelist used for filtering stack traces.
        /// If it is empty, all namespaces will be allowed.
        /// </summary>
        public List<string> NamespaceWhitelist { get; set; }

        /// <summary>
        /// Namespace blacklist used for filtering stack traces.
        /// </summary>
        public List<string> NamespaceBlacklist { get; set; }

        /// <summary>
        /// Messages that were sent to the output.
        /// </summary>
        public List<(object, LogLevel)> Messages { get; } = new List<(object, LogLevel)>();

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">Message to log. Can be an object, like an exception.</param>
        /// <param name="level">Log level to output with.</param>
        /// <param name="overridePostfix">Override postfix to use.</param>
        public void LogMessage(object message, LogLevel level, string overridePostfix)
        {
            this.Messages.Add((message, level));
        }
    }
    
    public class LogInitializeTest
    {
        /// <summary>
        /// Sets up the test by resetting the logs.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            Log.Logger = null;
            Log.SetMinimumLogLevel(LogLevel.Information);
        }
        
        /// <summary>
        /// Tests the Initialize method, then the SetMinimumLogLevel method.
        /// </summary>
        [Test]
        public void TestInitializeThenSetMinimumLogLevel()
        {
            // Check that the Logger doesn't exist by default.
            Assert.IsNull(Log.Logger);
            
            // Initialize the logger and check that it is set up.
            Log.Initialize("Test");
            var logger = Log.Logger;
            Assert.IsNotNull(logger);
            Assert.AreEqual(1, logger.Outputs.Count);

            var consoleOutput = (ConsoleOutput) logger.Outputs[0];
            Assert.IsTrue(consoleOutput.IncludeDate);
            Assert.AreEqual(LogLevel.Information, consoleOutput.MinimumLevel);
            Assert.AreEqual(new List<string>() { "Test" }, consoleOutput.AdditionalLogInfo);
            Assert.AreEqual(new List<string>() { "Construct." }, consoleOutput.NamespaceWhitelist);
            
            // Re-initialize the logger and check it hasn't changed.
            Log.Initialize("Test");
            Assert.AreSame(logger, Log.Logger);
            
            // Set the minimum log level and check that it changed.
            Log.SetMinimumLogLevel(LogLevel.Warning);
            Assert.AreEqual(LogLevel.Warning, consoleOutput.MinimumLevel);
            Log.SetMinimumLogLevel(LogLevel.Debug);
            Assert.AreEqual(LogLevel.Debug, consoleOutput.MinimumLevel);
        }
        
        /// <summary>
        /// Tests the SetMinimumLogLevel method, then the Initialize method.
        /// </summary>
        [Test]
        public void TestSetMinimumLogLevelThenInitialize()
        {
            // Initialize the logger with the minimum level set first and check that it is set up.
            Log.SetMinimumLogLevel(LogLevel.Warning);
            Log.Initialize("Test");
            var logger = Log.Logger;
            Assert.IsNotNull(logger);
            Assert.AreEqual(1, logger.Outputs.Count);

            var consoleOutput = (ConsoleOutput) logger.Outputs[0];
            Assert.IsTrue(consoleOutput.IncludeDate);
            Assert.AreEqual(LogLevel.Warning, consoleOutput.MinimumLevel);
            Assert.AreEqual(new List<string>() { "Test" }, consoleOutput.AdditionalLogInfo);
            Assert.AreEqual(new List<string>() { "Construct." }, consoleOutput.NamespaceWhitelist);
        }
    }

    public class LogOutputTest
    {
        /// <summary>
        /// Test output for the logging.
        /// </summary>
        private TestOutput _output;

        /// <summary>
        /// Sets up the test by setting up the logs.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._output = new TestOutput();
            Log.Initialize("Test");
            Log.SetMinimumLogLevel(LogLevel.Trace);
            Log.Logger.Outputs[0] = this._output;
        }

        /// <summary>
        /// Tests the Trace method.
        /// </summary>
        [Test]
        public void TestTrace()
        {
            Log.Trace("Test message 1");
            Log.Trace("Test message 2");
            Assert.AreEqual(2, this._output.Messages.Count);
            Assert.AreEqual(("Test message 1", LogLevel.Trace), this._output.Messages[0]);
            Assert.AreEqual(("Test message 2", LogLevel.Trace), this._output.Messages[1]);
        }

        /// <summary>
        /// Tests the Debug method.
        /// </summary>
        [Test]
        public void TestDebug()
        {
            Log.Debug("Test message 1");
            Log.Debug("Test message 2");
            Assert.AreEqual(2, this._output.Messages.Count);
            Assert.AreEqual(("Test message 1", LogLevel.Debug), this._output.Messages[0]);
            Assert.AreEqual(("Test message 2", LogLevel.Debug), this._output.Messages[1]);
        }

        /// <summary>
        /// Tests the Info method.
        /// </summary>
        [Test]
        public void TestInfo()
        {
            Log.Info("Test message 1");
            Log.Info("Test message 2");
            Assert.AreEqual(2, this._output.Messages.Count);
            Assert.AreEqual(("Test message 1", LogLevel.Information), this._output.Messages[0]);
            Assert.AreEqual(("Test message 2", LogLevel.Information), this._output.Messages[1]);
        }

        /// <summary>
        /// Tests the Warn method.
        /// </summary>
        [Test]
        public void TestWarn()
        {
            Log.Warn("Test message 1");
            Log.Warn("Test message 2");
            Assert.AreEqual(2, this._output.Messages.Count);
            Assert.AreEqual(("Test message 1", LogLevel.Warning), this._output.Messages[0]);
            Assert.AreEqual(("Test message 2", LogLevel.Warning), this._output.Messages[1]);
        }

        /// <summary>
        /// Tests the Trace method.
        /// </summary>
        [Test]
        public void TestError()
        {
            Log.Error("Test message 1");
            Log.Error("Test message 2");
            Assert.AreEqual(2, this._output.Messages.Count);
            Assert.AreEqual(("Test message 1", LogLevel.Error), this._output.Messages[0]);
            Assert.AreEqual(("Test message 2", LogLevel.Error), this._output.Messages[1]);
        }

        /// <summary>
        /// Tests the Critical method.
        /// </summary>
        [Test]
        public void TestCritical()
        {
            Log.Critical("Test message 1");
            Log.Critical("Test message 2");
            Assert.AreEqual(2, this._output.Messages.Count);
            Assert.AreEqual(("Test message 1", LogLevel.Critical), this._output.Messages[0]);
            Assert.AreEqual(("Test message 2", LogLevel.Critical), this._output.Messages[1]);
        }
    }
}