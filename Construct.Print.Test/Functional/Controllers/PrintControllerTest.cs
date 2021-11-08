using System;
using System.Collections.Generic;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Database.Model;
using Construct.Print.Controllers;
using Construct.Print.Data.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Construct.Print.Test.Functional.Controllers
{
    public class PrintControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private PrintController _printController;

        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            // Set up the controller.
            this._printController = new PrintController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            
            // Add the test users and test prints.
            this.AddData((context) =>
            {
                // Add the material.
                var testMaterial = new PrintMaterial()
                {
                    Name = "Test1",
                    CostPerGram = 0.03f,
                };
                context.PrintMaterials.Add(testMaterial);

                // Add the users.
                context.Users.Add(new User()
                {
                    HashedId = "test_hash_1",
                    Name = "Test Name 1",
                    Email = "test1@email",
                    SignUpTime = DateTime.Now,
                });
                context.Users.Add(new User()
                {
                    HashedId = "test_hash_2",
                    Name = "Test Name 2",
                    Email = "test2@email",
                    SignUpTime = DateTime.Now,
                    PrintLogs = new List<PrintLog>()
                    {
                        new PrintLog()
                        {
                            Time = DateTime.Now,
                            FileName = "Test File 1",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Purpose 1",
                            BillTo = "Test Bill To 1",
                            Cost = 0.3f,
                            Owed = true,
                        },
                        new PrintLog()
                        {
                            Time = DateTime.Now,
                            FileName = "Test File 2",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Purpose 2",
                            BillTo = "Test Bill To 2",
                            Cost = 0.6f,
                            Owed = false,
                        },
                    }
                });
            });
        }
        
        /// <summary>
        /// Tests the /print/last endpoint with a null HashedId.
        /// </summary>
        [Test]
        public void TestGetLastPrintNullHashedId()
        {
            var response = this._printController.GetLast(null).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/last endpoint with a empty HashedId.
        /// </summary>
        [Test]
        public void TestGetLastPrintEmptyHashedId()
        {
            var response = this._printController.GetLast("").Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/last endpoint with an unknown user.
        /// </summary>
        [Test]
        public void TestGetLastPrintUnknownUser()
        {
            var response = this._printController.GetLast("unknown_hash").Result.Value;
            Assert.AreEqual(404, this._printController.Response.StatusCode);
            Assert.AreEqual("user-not-found", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/last endpoint with a user with no prints.
        /// </summary>
        [Test]
        public void TestGetLastPrintNoPrints()
        {
            var response = this._printController.GetLast("test_hash_1").Result.Value;
            Assert.AreEqual(404, this._printController.Response.StatusCode);
            Assert.AreEqual("no-prints", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/last endpoint with a user with a last print.
        /// </summary>
        [Test]
        public void TestGetLastPrint()
        {
            var response = (LastPrintResponse) this._printController.GetLast("test_hash_2").Result.Value;
            Assert.AreEqual("success", response.Status);
            Assert.AreEqual("Test File 2", response.FileName);
            Assert.AreEqual("Test Purpose 2", response.Purpose);
            Assert.AreEqual("Test Bill To 2", response.BillTo);
        }
    }
}