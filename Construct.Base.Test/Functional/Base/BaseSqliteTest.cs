using System;
using System.IO;
using Construct.Core.Configuration;
using Construct.Core.Database.Context;
using NUnit.Framework;

namespace Construct.Base.Test.Functional.Base
{
    public class BaseSqliteTest
    {
        /// <summary>
        /// Location for the test database.
        /// </summary>
        private string _testDatabaseLocation;
        
        /// <summary>
        /// Sets up the test for creating the database.
        /// </summary>
        [SetUp]
        public void SetUpSqlite()
        {
            // Randomize the database location.
            this._testDatabaseLocation = "test-database-" + Guid.NewGuid() + ".sqlite";
                
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
        /// Adds data to the test database.
        /// </summary>
        /// <param name="addData">Action used to add data.</param>
        public void AddData(Action<ConstructContext> addData)
        {
            using var context = new ConstructContext();
            addData(context);
            context.SaveChanges();
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