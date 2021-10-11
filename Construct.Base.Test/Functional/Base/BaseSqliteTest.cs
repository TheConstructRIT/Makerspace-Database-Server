using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Construct.Core.Configuration;
using Construct.Core.Database.Context;
using Construct.Core.Receipt.Print;
using NUnit.Framework;
using RichardSzalay.MockHttp;

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
        /// Sets up the mocks for the PrintReceipts.
        /// </summary>
        [SetUp]
        public void SetUpPrintReceipts()
        {
            var mockClient = new MockHttpMessageHandler();
            mockClient.When(HttpMethod.Post, $"https://script.google.com/macros/s/valid_id/exec?request=sendemail&email=test@email&printCount=0&fileName=test.gcode&printWeight=10&printCost={0.3f}&totalCost={0f}")
                .Respond(HttpStatusCode.OK);
            mockClient.When(HttpMethod.Post, $"https://script.google.com/macros/s/invalid_id/exec?request=sendemail&email=test@email&printCount=0&fileName=test.gcode&printWeight=10&printCost={0.3f}&totalCost={0f}")
                .Respond(HttpStatusCode.NotFound);
            GoogleAppScriptPrintReceipt.Client = mockClient.ToHttpClient();
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