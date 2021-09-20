using System.Net;
using Construct.Base.Test.Integration.Base;
using Construct.Core.Data.Response;
using Construct.Swipe.Data.Request;
using Construct.User.Data.Request;
using NUnit.Framework;

namespace Construct.Combined.Test.Integration
{
    public class ProgramTest : BaseIntegrationTest
    {
        /// <summary>
        /// Starts the program.
        /// </summary>
        [SetUp]
        public void SetUpProgram()
        {
            this.StartProgram<Program>();
            this.WaitForApp("Combined");
        }

        /// <summary>
        /// Tests registering a user and adding a visit log.
        /// </summary>
        [Test]
        public void TestRegisterAndAddVisit()
        {
            // Register a user.
            var userRequest = new RegisterUserRequest()
            {
                HashedId = "test_hash",
                Name = "Test User",
                Email = "test@email",
                College = "Test College",
                Year = "Test Year",
            };
            var (registerResponse, registerResponseCode) = this.Post<BaseSuccessResponse>("Combined", "/user/register", userRequest);
            Assert.AreEqual(HttpStatusCode.OK, registerResponseCode);
            Assert.AreEqual("success",registerResponse.Status);
            
            // Add visit logs for the registered user.
            var addRequest = new AddRequest()
            {
                HashedId = "test_hash",
                Source = "TestSource"
            };
            var (addResponse, addResponseCode) = this.Post<GenericStatusResponse>("Combined", "/swipe/add", addRequest);
            Assert.AreEqual(HttpStatusCode.OK, addResponseCode);
            Assert.AreEqual("success",addResponse.Status);
            
            // Stop the program.
            // Can't be done in a TearDown since the ASP.NET app prevents it from running.
            this.StopProgram();
        }
    }
}