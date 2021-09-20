using System;
using System.Linq;
using System.Net;
using Construct.Base.Test.Integration.Base;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Swipe.Data.Request;
using NUnit.Framework;

namespace Construct.Swipe.Test.Integration.Controllers
{
    public class SwipeControllerTest : BaseIntegrationTest
    {
        /// <summary>
        /// Starts the program.
        /// </summary>
        [SetUp]
        public void SetUpProgram()
        {
            // Start the program.
            this.StartProgram<Program>();
            this.WaitForApp("Swipe");
            
            // Add a test user.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                });
            });
        }

        /// <summary>
        /// Tests adding a visit for a user.
        /// </summary>
        [Test]
        public void TestAddVisit()
        {
            // Add visit logs for the registered user.
            var addRequest = new AddRequest()
            {
                HashedId = "test_hash",
                Source = "TestSource"
            };
            var (addResponse, addResponseCode) = this.Post<GenericStatusResponse>("Swipe", "/swipe/add", addRequest);
            Assert.AreEqual(HttpStatusCode.OK, addResponseCode);
            Assert.AreEqual("success",addResponse.Status);
            this.Post<GenericStatusResponse>("Swipe", "/swipe/add", addRequest);
            Assert.AreEqual(HttpStatusCode.OK, addResponseCode);
            Assert.AreEqual("success",addResponse.Status);
            
            // Check that 2 visit logs were added.
            using var context = new ConstructContext();
            Assert.AreEqual(2, context.VisitLogs.ToList().Count);
            
            // Stop the program.
            // Can't be done in a TearDown since the ASP.NET app prevents it from running.
            this.StopProgram();
        }
        
        /// <summary>
        /// Tests adding a visit for a user not in the system.
        /// </summary>
        [Test]
        public void TestAddVisitUserNotFound()
        {
            // Add visit logs for the unknown user.
            var addRequest = new AddRequest()
            {
                HashedId = "unknown_hash",
                Source = "TestSource"
            };
            var (addResponse, addResponseCode) = this.Post<GenericStatusResponse>("Swipe", "/swipe/add", addRequest);
            Assert.AreEqual(HttpStatusCode.NotFound, addResponseCode);
            Assert.AreEqual("user-not-found",addResponse.Status);
            
            // Check that no visit logs were added.
            using var context = new ConstructContext();
            Assert.AreEqual(0, context.VisitLogs.ToList().Count);
            
            // Stop the program.
            // Can't be done in a TearDown since the ASP.NET app prevents it from running.
            this.StopProgram();
        }
    }
}