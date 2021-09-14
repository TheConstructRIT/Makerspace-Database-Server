using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Core.Configuration;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.Core.Test.Functional.Base;
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
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            ConstructConfiguration.Configuration.Email.ValidEmails = new List<string>() { "@email" };
            ConstructConfiguration.Configuration.Email.EmailCorrections = new Dictionary<string, string>() { {"@test.email", "@email"} };
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
        }

        /// <summary>
        /// Tests the /user/register endpoint with missing or invalid data.
        /// </summary>
        [Test]
        public void TestRegisterErrors()
        {
            // Create the base response.
            var request = new RegisterUserRequest()
            {
                HashedId = "test_hash",
                Name = "Test Name",
                Email = "test@email",
                College = "Test School",
                Year = "Test Year",
            };
            
            // Test the HashedId field.
            request.HashedId = null;
            var response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);

            request.HashedId = "";
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
            request.HashedId = "test_hash";
            
            // Test the Name field.
            request.Name = null;
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-name", response.Status);

            request.Name = "";
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-name", response.Status);
            request.Name = "Test Name";
            
            // Test the Email field.
            request.Email = null;
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-email", response.Status);

            request.Email = "";
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-email", response.Status);
            request.Email = "test@email";
            
            // Test the College field.
            request.College = null;
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-college", response.Status);

            request.College = "";
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-college", response.Status);
            request.College = "Test School";
            
            // Test the Year field.
            request.Year = null;
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-year", response.Status);

            request.Year = "";
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("missing-year", response.Status);
            request.Year = "Test Year";
            
            // Test invalid emails.
            request.Email = "test@invalid-email";
            this._userController.Response.StatusCode = 200;
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(400, this._userController.Response.StatusCode);
            Assert.AreEqual("invalid-email", response.Status);
        }

        /// <summary>
        /// Tests the /user/register endpoint.
        /// </summary>
        [Test]
        public void TestRegister()
        {
            // Create the base response.
            var request = new RegisterUserRequest()
            {
                HashedId = "test_hash",
                Name = "Test Name",
                Email = "test@test.email",
                College = "Test School",
                Year = "Test Year",
            };

            // Send a request and check that it was registered.
            var response = this._userController.Register(request).Result.Value;
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
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(409, this._userController.Response.StatusCode);
            Assert.AreEqual("duplicate-user", response.Status);
            
            // Register another email and check the Student entries have different keys.
            this._userController.Response.StatusCode = 200;
            request.HashedId += "_2";
            response = this._userController.Register(request).Result.Value;
            Assert.AreEqual(200, this._userController.Response.StatusCode);
            Assert.AreEqual("success", response.Status);

            var students = context.Students.ToList();
            Assert.AreEqual(2, students.Count);
            Assert.AreNotEqual(students[0].Key, students[1].Key);
        }
    }
}