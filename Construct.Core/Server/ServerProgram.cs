using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .Build().Run();;
        }
    }
}