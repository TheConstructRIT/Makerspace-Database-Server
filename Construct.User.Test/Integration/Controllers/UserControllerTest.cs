using System.Net;
using Construct.Base.Test.Integration.Base;
using Construct.Core.Data.Response;
using Construct.User.Data.Request;
using Construct.User.Data.Response;
using NUnit.Framework;

namespace Construct.User.Test.Integration.Controllers
{
    public class UserControllerTest : BaseIntegrationTest
    {
        /// <summary>
        /// Starts the program.
        /// </summary>
        [SetUp]
        public void SetUpProgram()
        {
            this.StartProgram<Program>();
            this.WaitForApp("User");
        }
        
        /// <summary>
        /// Tests registering and getting a user.
        /// </summary>
        [Test]
        public void TestRegisterUser()
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
            var (registerResponse, registerResponseCode) = this.Post<BaseSuccessResponse>("User", "/user/register", userRequest);
            Assert.AreEqual(HttpStatusCode.OK, registerResponseCode);
            Assert.AreEqual("success",registerResponse.Status);
            
            // Get the registered user.
            var (getResponse, getResponseCode) = this.Get<GetUserResponse>("User", "/user/get?hashedid=test_hash");
            Assert.AreEqual(HttpStatusCode.OK, getResponseCode);
            Assert.AreEqual("success", getResponse.Status);
            Assert.AreEqual("Test User", getResponse.Name);
            Assert.AreEqual("test@email", getResponse.Email);
            Assert.AreEqual(0, getResponse.OwedPrintBalance);
            Assert.AreEqual(0, getResponse.Permissions.Count);
            
            // Stop the program.
            // Can't be done in a TearDown since the ASP.NET app prevents it from running.
            this.StopProgram();
        }
        
        /// <summary>
        /// Tests registering duplicate users.
        /// </summary>
        [Test]
        public void TestRegisterUserDuplicate()
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
            var (registerResponse, registerResponseCode) = this.Post<GenericStatusResponse>("User", "/user/register", userRequest);
            Assert.AreEqual(HttpStatusCode.OK, registerResponseCode);
            Assert.AreEqual("success",registerResponse.Status);
            
            // Re-send the request and check that HTTP 409 Conflict is returned.
            (registerResponse, registerResponseCode) = this.Post<GenericStatusResponse>("User", "/user/register", userRequest);
            Assert.AreEqual(HttpStatusCode.Conflict, registerResponseCode);
            Assert.AreEqual("duplicate-user", registerResponse.Status);
            
            // Stop the program.
            // Can't be done in a TearDown since the ASP.NET app prevents it from running.
            this.StopProgram();
        }
        
        /// <summary>
        /// Tests getting a user that isn't registered.
        /// </summary>
        [Test]
        public void TestGetUserNotFound()
        {
            // Get the user and check that it was not found.
            var (getResponse, getResponseCode) = this.Get<GenericStatusResponse>("User", "/user/get?hashedid=test_hash");
            Assert.AreEqual(HttpStatusCode.NotFound, getResponseCode);
            Assert.AreEqual("not-found", getResponse.Status);
            
            // Stop the program.
            // Can't be done in a TearDown since the ASP.NET app prevents it from running.
            this.StopProgram();
        }
    }
}