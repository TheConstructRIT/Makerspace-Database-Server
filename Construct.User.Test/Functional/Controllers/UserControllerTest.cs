using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Core.Configuration;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.Base.Test.Functional.Base;
using Construct.User.Controllers;
using Construct.User.Data.Request;
using Construct.User.Data.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Construct.User.Test.Functional.Controllers
{
    public class UserControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private UserController _userController;

        /// <summary>
        /// Register user request used for the tests.
        /// </summary>
        private RegisterUserRequest _registerUserRequest;
        
        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            this._registerUserRequest = new RegisterUserRequest()
            {
                HashedId = "test_hash",
                Name = "Test Name",
                Email = "test@email",
                College = "Test School",
                Year = "Test Year",
            };
            
            ConstructConfiguration.Configuration.Email.ValidEmails = new List<string>() { "email" };
            ConstructConfiguration.Configuration.Email.EmailCorrections = new Dictionary<string, string>() { {"test.email", "email"} };
            this._userController = new UserController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }
        
        /// <summary>
        /// Tests the /user/get endpoint with the user not found.
        /// </summary>
        [Test]
        public void TestGetNotFound()
        {
            // Run a request and make sure it returned a not found request.
            var response = _userController.Get("not_found").Result.Value;
            Assert.AreEqual(404, this._userController.Response.StatusCode);
            Assert.IsTrue(response is NotFoundResponse);
            Assert.AreEqual("not-found", response.Status);
        }
        
        /// <summary>
        /// Tests the /user/get endpoint with a user with no prints.
        /// </summary>
        [Test]
        public void TestGetNoPrints()
        {
            // Add the user.
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
            
            // Run a request and make sure it returned a not found request.
            var response = _userController.Get("test_hash").Result.Value;
            Assert.AreEqual(200, this._userController.Response.StatusCode);
            var userResponse = (GetUserResponse) response;
            Assert.AreEqual("success", userResponse.Status);
            Assert.AreEqual("Test Name", userResponse.Name);
            Assert.AreEqual("test@email", userResponse.Email);
            Assert.AreEqual(0, userResponse.OwedPrintBalance);
            Assert.AreEqual(0, userResponse.Permissions.Count);
        }
        
        /// <summary>
        /// Tests the /user/get endpoint with a user with no owed prints.
        /// </summary>
        [Test]
        public void TestGetNoOwedPrints()
        {
            // Add the user.
            this.AddData((context) =>
            {
                // Add the material.
                var testMaterial = new PrintMaterial()
                {
                    Name = "Test",
                    CostPerGram = 0.03f,
                };
                context.PrintMaterials.Add(testMaterial);
                
                // Add the user.
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                    PrintLogs = new List<PrintLog>()
                    {
                        new PrintLog()
                        {
                            Time = DateTime.Now,
                            FileName = "Test File 1",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Print",
                            BillTo = "Test Bill To",
                            Cost = 0.3f,
                            Owed = false,
                        },
                        new PrintLog()
                        {
                            Time = DateTime.Now,
                            FileName = "Test File 2",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Print",
                            BillTo = "Test Bill To",
                            Cost = 0.6f,
                            Owed = false,
                        },
                        new PrintLog()
                        {
                            Time = DateTime.Now,
                            FileName = "Test File 3",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Print",
                            BillTo = "Test Bill To",
                            Cost = 0.2f,
                            Owed = false,
                        },
                    }
                });
            });
            
            // Run a request and make sure it returned a not found request.
            var response = _userController.Get("test_hash").Result.Value;
            Assert.AreEqual(200, this._userController.Response.StatusCode);
            var userResponse = (GetUserResponse) response;
            Assert.AreEqual("success", userResponse.Status);
            Assert.AreEqual("Test Name", userResponse.Name);
            Assert.AreEqual("test@email", userResponse.Email);
            Assert.AreEqual(0, userResponse.OwedPrintBalance);
            Assert.AreEqual(0, userResponse.Permissions.Count);
        }
        
        /// <summary>
        /// Tests the /user/get endpoint with a user with some owed prints.
        /// </summary>
        [Test]
        public void TestGetSomeOwedPrints()
        {
            // Add the user.
            this.AddData((context) =>
            {
                // Add the material.
                var testMaterial = new PrintMaterial()
                {
                    Name = "Test",
                    CostPerGram = 0.03f,
                };
                context.PrintMaterials.Add(testMaterial);
                
                // Add the user.
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                    PrintLogs = new List<PrintLog>()
                    {
                        new PrintLog()
                        {
                            Time = DateTime.Now,
                            FileName = "Test File 1",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Print",
                            BillTo = "Test Bill To",
                            Cost = 0.3f,
                            Owed = true,
                        },
                        new PrintLog()
                        {
                            Time = DateTime.Now,
                            FileName = "Test File 2",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Print",
                            BillTo = "Test Bill To",
                            Cost = 0.6f,
                            Owed = false,
                        },
                        new PrintLog()
                        {
                            Time = DateTime.Now,
                            FileName = "Test File 3",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Print",
                            BillTo = "Test Bill To",
                            Cost = 0.2f,
                            Owed = true,
                        },
                    }
                });
            });
            
            // Run a request and make sure it returned a not found request.
            var response = _userController.Get("test_hash").Result.Value;
            Assert.AreEqual(200, this._userController.Response.StatusCode);
            var userResponse = (GetUserResponse) response;
            Assert.AreEqual("success", userResponse.Status);
            Assert.AreEqual("Test Name", userResponse.Name);
            Assert.AreEqual("test@email", userResponse.Email);
            Assert.That(Math.Abs(userResponse.OwedPrintBalance - 0.5) < 0.001);
            Assert.AreEqual(0, userResponse.Permissions.Count);
        }
        
        /// <summary>
        /// Tests the /user/get endpoint with a user with permissions.
        /// </summary>
        [Test]
        public void TestGetPermissions()
        {
            // Add the user.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                    Permissions = new List<Permission>()
                    {
                        new Permission()
                        {
                            Name = "Active1",
                        },
                        new Permission()
                        {
                            Name = "Active2",
                            StartTime = DateTime.Now.AddSeconds(-10),
                        },
                        new Permission()
                        {
                            Name = "Active3",
                            EndTime = DateTime.Now.AddSeconds(10),
                        },
                        new Permission()
                        {
                            Name = "Expired1",
                            StartTime = DateTime.Now.AddSeconds(10),
                        },
                        new Permission()
                        {
                            Name = "Expired2",
                            EndTime = DateTime.Now.AddSeconds(-10),
                        },
                    }
                });
            });
            
            // Run a request and make sure it returned a not found request.
            var response = _userController.Get("test_hash").Result.Value;
            Assert.AreEqual(200, this._userController.Response.StatusCode);
            var userResponse = (GetUserResponse) response;
            Assert.AreEqual("success", userResponse.Status);
            Assert.AreEqual("Test Name", userResponse.Name);
            Assert.AreEqual("test@email", userResponse.Email);
            Assert.AreEqual(0, userResponse.OwedPrintBalance);
            Assert.AreEqual(new List<string>() { "Active1", "Active2", "Active3" }, userResponse.Permissions);
        }

        /// <summary>
        /// Tests the /user/register endpoint with a null HashedId.
        /// </summary>
        [Test]
        public void TestRegisterNullHashedId()
        {
            this._registerUserRequest.HashedId = null;
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with an empty HashedId.
        /// </summary>
        [Test]
        public void TestRegisterEmptyHashedId()
        {
            this._registerUserRequest.HashedId = "";
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with a null Name.
        /// </summary>
        [Test]
        public void TestRegisterNullName()
        {
            this._registerUserRequest.Name = null;
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-name", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with an empty Name.
        /// </summary>
        [Test]
        public void TestRegisterEmptyName()
        {
            this._registerUserRequest.Name = "";
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-name", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with a null Email.
        /// </summary>
        [Test]
        public void TestRegisterNullEmail()
        {
            this._registerUserRequest.Email = null;
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-email", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with an empty Email.
        /// </summary>
        [Test]
        public void TestRegisterEmptyEmail()
        {
            this._registerUserRequest.Email = "";
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-email", response.Status);
        }
        
        /// <summary>
        /// Tests the /user/register endpoint with an invalid Email.
        /// </summary>
        [Test]
        public void TestRegisterInvalidEmail()
        {
            this._registerUserRequest.Email = "test@invalid-email";
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("invalid-email", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with a null College.
        /// </summary>
        [Test]
        public void TestRegisterNullCollege()
        {
            this._registerUserRequest.College = null;
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-college", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with an empty College.
        /// </summary>
        [Test]
        public void TestRegisterEmptyCollege()
        {
            this._registerUserRequest.College = "";
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-college", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with a null Year.
        /// </summary>
        [Test]
        public void TestRegisterNullYear()
        {
            this._registerUserRequest.Year = null;
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-year", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint with an empty Year.
        /// </summary>
        [Test]
        public void TestRegisterEmptyYear()
        {
            this._registerUserRequest.Year = "";
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-year", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint.
        /// </summary>
        [Test]
        public void TestRegister()
        {
            // Send a request and check that it was registered.
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(200, this._userController.Response.StatusCode);
            Assert.AreEqual("success", response.Status);
            
            // Test that the user was created.
            using var context = new ConstructContext();
            var student = context.Students.Include(s => s.User).First();
            Assert.AreEqual("Test School", student.College);
            Assert.AreEqual("Test Year", student.Year);
            Assert.AreEqual("test_hash", student.User.HashedId);
            Assert.AreEqual("Test Name", student.User.Name);
            Assert.AreEqual("test@email", student.User.Email);
            Assert.That(student.User.SignUpTime.HasValue && ((DateTimeOffset) student.User.SignUpTime).ToUnixTimeSeconds() + 10 > DateTimeOffset.Now.ToUnixTimeSeconds());
            
            // Re-send the request and check it was rejected.
            response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(409, this._userController.Response.StatusCode);
            Assert.AreEqual("duplicate-user", response.Status);
            
            // Register another email and check the Student entries have different keys.
            this._userController.Response.StatusCode = 200;
            this._registerUserRequest.HashedId += "_2";
            this._registerUserRequest.Email = "test2@email";
            response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(200, this._userController.Response.StatusCode);
            Assert.AreEqual("success", response.Status);

            var students = context.Students.ToList();
            Assert.AreEqual(2, students.Count);
            Assert.AreNotEqual(students[0].Key, students[1].Key);
        }
        
        /// <summary>
        /// Tests the /user/register endpoint with duplicate emails.
        /// </summary>
        [Test]
        public void TestRegisterDuplicateEmails()
        {
            // Send a request and check that it was registered.
            var response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(200, this._userController.Response.StatusCode);
            Assert.AreEqual("success", response.Status);
            
            // Re-send the request and check it was rejected.
            this._registerUserRequest.HashedId += "_2";
            response = this._userController.Register(this._registerUserRequest).Result.Value;
            Assert.AreEqual(409, this._userController.Response.StatusCode);
            Assert.AreEqual("duplicate-user", response.Status);
            
            // Test that the user was created.
            using var context = new ConstructContext();
            var student = context.Students.Include(s => s.User).First();
            Assert.AreEqual("Test School", student.College);
            Assert.AreEqual("Test Year", student.Year);
            Assert.AreEqual("test_hash", student.User.HashedId);
            Assert.AreEqual("Test Name", student.User.Name);
            Assert.AreEqual("test@email", student.User.Email);
            Assert.That(student.User.SignUpTime.HasValue && ((DateTimeOffset) student.User.SignUpTime).ToUnixTimeSeconds() + 10 > DateTimeOffset.Now.ToUnixTimeSeconds());
        }
    }
}