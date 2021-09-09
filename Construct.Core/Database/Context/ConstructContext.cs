using System;
using System.Threading.Tasks;
using Construct.Core.Configuration;
using Construct.Core.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Construct.Core.Database.Context
{
    public class ConstructContext : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Users in the database.
        /// </summary>
        public DbSet<User> Users => this.WrappedContext.Users;

        /// <summary>
        /// Permissions in the database.
        /// </summary>
        public DbSet<Permission> Permissions => this.WrappedContext.Permissions;

        /// <summary>
        /// Visit logs in the database.
        /// </summary>
        public DbSet<VisitLog> VisitLogs => this.WrappedContext.VisitLogs;

        /// <summary>
        /// Print logs in the database.
        /// </summary>
        public DbSet<PrintLog> PrintLog => this.WrappedContext.PrintLog;

        /// <summary>
        /// Print materials in the database.
        /// </summary>
        public DbSet<PrintMaterial> PrintMaterials => this.WrappedContext.PrintMaterials;
        
        /// <summary>
        /// Wrapped context used depending on the configuration.
        /// </summary>
        private readonly BaseContext WrappedContext;

        /// <summary>
        /// Creates the Construct Context.
        /// </summary>
        public ConstructContext()
        {
            // Get the provider.
            var provider = ConstructConfiguration.Configuration.Database.Provider.ToLower();
            Log.Trace($"Getting context with provider {provider}");
            
            // Set the context.
            if (provider == "sqlite")
            {
                this.WrappedContext = new SqliteContext();
            }
            else
            {
                Log.Error($"Unsupported database provider: {provider}");
                throw new InvalidOperationException($"Unsupported database provider: {provider}");
            }
        }
        
        /// <summary>
        /// Ensures that the database is migrated to the latest version.
        /// </summary>
        public async Task EnsureUpToDateAsync()
        {
            await this.WrappedContext.EnsureUpToDateAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        public void SaveChanges() => this.WrappedContext.SaveChanges();

        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        public Task SaveChangesAsync() => this.WrappedContext.SaveChangesAsync();
        
        
        /// <summary>
        /// Disposes of the context.
        /// </summary>
        public ValueTask DisposeAsync() => this.WrappedContext.DisposeAsync();

        /// <summary>
        /// Disposes of the context.
        /// </summary>
        public void Dispose() => this.WrappedContext?.Dispose();
    }
}