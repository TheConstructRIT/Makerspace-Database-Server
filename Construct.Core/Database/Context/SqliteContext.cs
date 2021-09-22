using Construct.Core.Configuration;
using Construct.Core.Logging;
using Microsoft.EntityFrameworkCore;

namespace Construct.Core.Database.Context
{
    public class SqliteContext : BaseContext
    {
        /// <summary>
        /// Flag for the initial source output warning being outputted.
        /// </summary>
        private static bool _warningOutput = false;
        
        /// <summary>
        /// Configures the context.
        /// </summary>
        /// <param name="optionsBuilder">Builder for the context options.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Get the source.
            var source = "database.sqlite";
            if (ConstructConfiguration.Configuration.Database.Source != null)
            {
                source = ConstructConfiguration.Configuration.Database.Source;
            }
            else if (!_warningOutput)
            {
                _warningOutput = true;
                Log.Warn($"Database source not set for Sqlite. Defaulting to {source}");
            }
            
            // Set up the source.
            optionsBuilder.UseSqlite($"Data Source=\"{source}\"");
        }
    }
}