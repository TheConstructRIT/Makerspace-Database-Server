using System;
using System.Collections.Generic;
using System.Net;
using Construct.Admin.Data.Response;
using Construct.Base.Test.Integration.Base;
using Construct.Core.Configuration;
using Construct.Core.Data.Response;
using Construct.Core.Database.Model;
using NUnit.Framework;

namespace Construct.Admin.Test.Integration
{
    public class AdminSessionControllerTest : BaseIntegrationTest
    {
        /// <summary>
        /// Starts the program.
        /// </summary>
        [SetUp]
        public void SetUpProgram()
        {
            // Start the program.
            ConstructConfiguration.Configuration.Admin.MaximumUserSessions = 3;
            this.StartProgram<Program>();
            this.WaitForApp("Admin");
            
            // Add the test users.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash_1",
                    Name = "Test Name 1",
                    Email = "test1@email",
                    SignUpTime = DateTime.Now,
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash_2",
                    Name = "Test Name 2",
                    Email = "test2@email",
                    SignUpTime = DateTime.Now,
                    Permissions = new List<Permission>()
                    {
                        new Permission()
                        {
                            Name = "LabManager",
                        }
                    },
                });
            });
        }

        /// <summary>
        /// Tests creating a session with an unauthorized user.
        /// </summary>
        [Test]
        public void TestCreateSessionUnauthorized()
        {
            var (authenticateResponse, authenticateResponseCode) = this.Get<UnauthorizedResponse>("Admin", "/admin/authenticate?hashedid=test_hash_1");
            Assert.AreEqual(HttpStatusCode.Unauthorized, authenticateResponseCode);
            Assert.AreEqual("unauthorized",authenticateResponse.Status);
        }

        /// <summary>
        /// Tests creating a session with an authorized user.
        /// </summary>
        [Test]
        public void TestCreateSessionAuthorized()
        {
            // Test getting the session.
            var (authenticateResponse, authenticateResponseCode) = this.Get<SessionResponse>("Admin", "/admin/authenticate?hashedid=test_hash_2");
            Assert.AreEqual(HttpStatusCode.OK, authenticateResponseCode);
            Assert.AreEqual("success",authenticateResponse.Status);
            Assert.IsNotEmpty(authenticateResponse.Session);
            
            // Test checking the session is valid.
            var (checkResponse, checkResponseCode) = this.Get<BaseSuccessResponse>("Admin", "/admin/checksession?session=" + authenticateResponse.Session);
            Assert.AreEqual(HttpStatusCode.OK, checkResponseCode);
            Assert.AreEqual("success",checkResponse.Status);
            
            // Test expiring the session.
            for (var i = 0; i < 3; i++)
            {
                this.Get<SessionResponse>("Admin", "/admin/authenticate?hashedid=test_hash_2");
            }
            var (newCheckResponse, newCheckResponseCode) = this.Get<UnauthorizedResponse>("Admin", "/admin/checksession?session=" + authenticateResponse.Session);
            Assert.AreEqual(HttpStatusCode.Unauthorized, newCheckResponseCode);
            Assert.AreEqual("unauthorized",newCheckResponse.Status);
        }
    }
}