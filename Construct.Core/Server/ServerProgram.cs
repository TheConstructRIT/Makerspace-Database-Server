using System;
using Construct.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Construct.Core.Server
{
    public class ServerProgram
    {
        /// <summary>
        /// Runs the server program.
        /// </summary>
        /// <param name="args">Arguments from the command line.</param>
        /// <param name="identifier">Name of the application domain.</param>
        public static void Run(string[] args, string identifier)
        {
            // Set up the logging.
            Log.Initialize(identifier);
            
            // Load the configuration.
            ConstructConfiguration.LoadDefaultAsync().Wait();
            Log.SetMinimumLogLevel(ConstructConfiguration.Configuration.Logging?.ConsoleLevel ?? LogLevel.Information);
            
            // Get the port.
            if (!ConstructConfiguration.Configuration.Ports.ContainsKey(identifier))
            {
                Log.Critical("Port not defined for " + identifier);
                Environment.Exit(-1);
            }
            var port = ConstructConfiguration.Configuration.Ports[identifier];
            
            // Build the app.
            Log.Debug("Preparing server.");
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => logging.ClearProviders())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://*:" + port);
                })
                .Build();
            
            // Start the server.
            Log.Info("Starting server on port " + port + ".");
            host.Run();
        }
    }
}