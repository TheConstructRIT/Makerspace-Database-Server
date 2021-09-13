using System;
using System.Collections.Generic;
using Construct.Core.Data.Response;
using Construct.Core.Database.Model;
using Construct.Core.Test.Integration.Base;
using Construct.User.Controllers;
using Construct.User.Data.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Construct.User.Test.Integration.Controllers
{
    public class UserControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private readonly UserController _userController = new UserController()
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        
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
    }
}