using Construct.Core.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Construct.Core.Database.Context
{
    public class PostgresContext : BaseContext
    {
        /// <summary>
        /// Configures the context.
        /// </summary>
        /// <param name="optionsBuilder">Builder for the context options.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Get the source.
            // The default source is set up for the CreateMigrate.py script.
            var dataSource = "Host=localhost:39468";
            if (ConstructConfiguration.Configuration.Database.Source != null)
            {
                dataSource = $"Host={ConstructConfiguration.Configuration.Database.Source};" +
                             (ConstructConfiguration.Configuration.Database.SourcePort.HasValue ? $"Port={ConstructConfiguration.Configuration.Database.SourcePort};" : "")+
                             $"Database={ConstructConfiguration.Configuration.Database.SourceDatabase};" +
                             $"Username={ConstructConfiguration.Configuration.Database.Username};" +
                             $"Password={ConstructConfiguration.Configuration.Database.Password}";
            }
            
            // Set up the source.
            optionsBuilder.UseNpgsql(dataSource);
        }
    }
}