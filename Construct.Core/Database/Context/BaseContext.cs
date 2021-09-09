using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Construct.Core.Database.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nexus.Logging.Entry;

namespace Construct.Core.Database.Context
{
    public abstract class BaseContext : DbContext, IAsyncDisposable
    {
        /// <summary>
        /// Users in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }
        
        /// <summary>
        /// Permissions in the database.
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Visit logs in the database.
        /// </summary>
        public DbSet<VisitLog> VisitLogs { get; set; }

        /// <summary>
        /// Print logs in the database.
        /// </summary>
        public DbSet<PrintLog> PrintLog { get; set; }

        /// <summary>
        /// Print materials in the database.
        /// </summary>
        public DbSet<PrintMaterial> PrintMaterials { get; set; }
        
        /// <summary>
        /// Ensures that the database is migrated to the latest version.
        /// </summary>
        public async Task EnsureUpToDateAsync()
        {
            await Database.MigrateAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Disposes of the context.
        /// </summary>
        public override ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
    }
}