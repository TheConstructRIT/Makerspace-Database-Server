using System;
using System.IO;
using Construct.Core.Configuration;
using Construct.Core.Database.Context;
using NUnit.Framework;

namespace Construct.Core.Test.Integration.Base
{
    public class BaseSqliteTest
    {
        /// <summary>
        /// Location for the test database.
        /// </summary>
        private readonly string _testDatabaseLocation = "test-database-" + Guid.NewGuid() + ".sqlite";
        
        /// <summary>
        /// Sets up the test for creating the database.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Set up the configuration.
            ConstructConfiguration.Configuration.Database.Provider = "sqlite";
            ConstructConfiguration.Configuration.Database.Source = this._testDatabaseLocation;
            
            // Remove the existing database file if one exists.
            if (File.Exists(this._testDatabaseLocation))
            {
                File.Delete(this._testDatabaseLocation);
            }
            
            // Set up the database.
            using var context = new ConstructContext();
            context.EnsureUpToDateAsync().Wait();
        }

        /// <summary>
        /// Tears down the test by clearing the test database.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // Remove the existing database file if one exists.
            if (File.Exists(this._testDatabaseLocation))
            {
                File.Delete(this._testDatabaseLocation);
            }
        }
    }
}