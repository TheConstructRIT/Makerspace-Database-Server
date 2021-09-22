using System;
using System.Threading.Tasks;
using Construct.Core.Configuration;
using Construct.Core.Database.Model;
using Construct.Core.Logging;
using Microsoft.EntityFrameworkCore;

namespace Construct.Core.Database.Context
{
    public class ConstructContext : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Users in the database.
        /// </summary>
        public DbSet<User> Users => this._wrappedContext.Users;
        
        /// <summary>
        /// Students in the database.
        /// </summary>
        public DbSet<Student> Students => this._wrappedContext.Students;

        /// <summary>
        /// Permissions in the database.
        /// </summary>
        public DbSet<Permission> Permissions => this._wrappedContext.Permissions;

        /// <summary>
        /// Visit logs in the database.
        /// </summary>
        public DbSet<VisitLog> VisitLogs => this._wrappedContext.VisitLogs;

        /// <summary>
        /// Print logs in the database.
        /// </summary>
        public DbSet<PrintLog> PrintLog => this._wrappedContext.PrintLog;

        /// <summary>
        /// Print materials in the database.
        /// </summary>
        public DbSet<PrintMaterial> PrintMaterials => this._wrappedContext.PrintMaterials;
        
        /// <summary>
        /// Wrapped context used depending on the configuration.
        /// </summary>
        private readonly BaseContext _wrappedContext;

        /// <summary>
        /// Creates the Construct Context.
        /// </summary>
        public ConstructContext()
        {
            // Get the provider.
            var provider = ConstructConfiguration.Configuration.Database.Provider?.ToLower();
            Log.Trace($"Getting context with provider {provider}");
            
            // Set the context.
            if (provider == "sqlite")
            {
                this._wrappedContext = new SqliteContext();
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
            await this._wrappedContext.EnsureUpToDateAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        public void SaveChanges() => this._wrappedContext.SaveChanges();

        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        public Task SaveChangesAsync() => this._wrappedContext.SaveChangesAsync();
        
        
        /// <summary>
        /// Disposes of the context.
        /// </summary>
        public ValueTask DisposeAsync() => this._wrappedContext.DisposeAsync();

        /// <summary>
        /// Disposes of the context.
        /// </summary>
        public void Dispose() => this._wrappedContext?.Dispose();
    }
}