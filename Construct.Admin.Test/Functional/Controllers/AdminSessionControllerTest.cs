using System;
using System.Collections.Generic;
using Construct.Admin.Controllers;
using Construct.Admin.Data.Response;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Data.Response;
using Construct.Core.Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Construct.Admin.Test.Functional.Controllers
{
    public class AdminSessionControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private AdminSessionController _adminSessionController;
        
        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            // Create the controller.
            this._adminSessionController = new AdminSessionController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            
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
                            Name = "NotLabManager",
                        },
                    },
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash_3",
                    Name = "Test Name 3",
                    Email = "test3@email",
                    SignUpTime = DateTime.Now,
                    Permissions = new List<Permission>()
                    {
                        new Permission()
                        {
                            Name = "LabManager",
                        },
                    },
                });
            });
        }

        /// <summary>
        /// Tests the GetAuthenticate with no hashed id.
        /// </summary>
        [Test]
        public void TestGetAuthenticateNoHashedId()
        {
            var response = this._adminSessionController.GetAuthenticate(null).Result.Value;
            Assert.AreEqual(400, this._adminSessionController.Response.StatusCode);
            Assert.IsTrue(response is GenericStatusResponse);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }

        /// <summary>
        /// Tests the GetAuthenticate with no user.
        /// </summary>
        [Test]
        public void TestGetAuthenticateNoUser()
        {
            var response = this._adminSessionController.GetAuthenticate("unknown_user").Result.Value;
            Assert.AreEqual(401, this._adminSessionController.Response.StatusCode);
            Assert.IsTrue(response is UnauthorizedResponse);
        }

        /// <summary>
        /// Tests the GetAuthenticate with a user with no permissions.
        /// </summary>
        [Test]
        public void TestGetAuthenticateNoPermissions()
        {
            var response = this._adminSessionController.GetAuthenticate("test_hash_1").Result.Value;
            Assert.AreEqual(401, this._adminSessionController.Response.StatusCode);
            Assert.IsTrue(response is UnauthorizedResponse);
        }

        /// <summary>
        /// Tests the GetAuthenticate with no lab manager permissions.
        /// </summary>
        [Test]
        public void TestGetAuthenticateNotLabManager()
        {
            var response = this._adminSessionController.GetAuthenticate("test_hash_2").Result.Value;
            Assert.AreEqual(401, this._adminSessionController.Response.StatusCode);
            Assert.IsTrue(response is UnauthorizedResponse);
        }

        /// <summary>
        /// Tests the GetAuthenticate with a lab manager.
        /// </summary>
        [Test]
        public void TestGetAuthenticateLabManager()
        {
            var response = (SessionResponse) this._adminSessionController.GetAuthenticate("test_hash_3").Result.Value;
            Assert.AreEqual(200, this._adminSessionController.Response.StatusCode);
            var nextResponse = (SessionResponse) this._adminSessionController.GetAuthenticate("test_hash_3").Result.Value;
            Assert.AreNotEqual(response.Session, nextResponse.Session);

        }
    }
}