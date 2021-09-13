using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Construct.Core.Configuration;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Construct.Core.Test.Functional.Database.Context
{
    public class ConstructContextCommonTest
    {
        /// <summary>
        /// Tests the ConstructContext.
        /// The configuration must be set up before running.
        /// </summary>
        public static async Task TestContextAsync()
        {
            // Create the context and make sure it is up to date.
            // If a migration fails or saving fails, the test will stop.
            await using var context = new ConstructContext();
            await context.EnsureUpToDateAsync();
            
            // Write some models and save the data.
            var startTime = DateTime.Now;
            var user = new User()
            {
                HashedId = "test",
                Name = "Test User",
                Email = "test@test.com",
                SignUpTime = startTime,
            };
            user.Permissions = new List<Permission>()
            {
                new Permission()
                {
                    User = user,
                    Name = "Permission 1"
                },
                new Permission()
                {
                    User = user,
                    Name = "Permission 2"
                },
            };
            context.Users.Add(user);
            context.VisitLogs.Add(new VisitLog()
            {
                User = user,
                Source = "Test System",
                Time = startTime,
            });
            await context.SaveChangesAsync();
            
            // Create a new context and check the stored user is correct.
            await using var readContext = new ConstructContext();
            var visitLog = readContext.VisitLogs.Include(c => c.User)
                .ThenInclude(c => c.Permissions)
                .First(log => log.User.HashedId == "test");
            Assert.AreEqual("Test System", visitLog.Source);
            Assert.AreEqual(startTime, visitLog.Time);
            Assert.AreEqual("test", visitLog.User.HashedId);
            Assert.AreEqual("Test User", visitLog.User.Name);
            Assert.AreEqual("test@test.com", visitLog.User.Email);
            Assert.AreEqual(2, visitLog.User.Permissions.Count);
            Assert.AreEqual("Permission 1", visitLog.User.Permissions[0].Name);
            Assert.AreEqual("Permission 2", visitLog.User.Permissions[1].Name);
            Assert.AreEqual(startTime, visitLog.User.SignUpTime);
        }
    }
    
    public class ConstructContextSqliteTest : ConstructContextCommonTest
    {
        /// <summary>
        /// Location for the test database.
        /// </summary>
        private const string TestDatabaseLocation = "test-database.sqlite";
        
        /// <summary>
        /// Sets up the test for creating the database.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Set up the configuration.
            ConstructConfiguration.Configuration.Database.Provider = "sqlite";
            ConstructConfiguration.Configuration.Database.Source = TestDatabaseLocation;
            
            // Remove the existing database file if one exists.
            if (File.Exists(TestDatabaseLocation))
            {
                File.Delete(TestDatabaseLocation);
            }
        }

        /// <summary>
        /// Tears down the test by clearing the test database.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // Remove the existing database file if one exists.
            if (File.Exists(TestDatabaseLocation))
            {
                File.Delete(TestDatabaseLocation);
            }
        }
        
        /// <summary>
        /// Tests writing and reading data.
        /// </summary>
        [Test]
        public async Task TestContext()
        {
            await TestContextAsync();
        }
    }
}