using System;
using Construct.Core.Server;

namespace Construct.Compatibility
{
    public class Program
    {
        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="args">Arguments from the command line.</param>
        public static void Main(string[] args) => ServerProgram.Run(args, "Compatibility");
    }
}