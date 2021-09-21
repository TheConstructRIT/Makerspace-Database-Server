using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Base.Test.Integration.Base;
using Construct.Compatibility.Controllers;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Construct.Compatibility.Test.Functional.Controllers
{
    public class CompatibilityControllerTest : BaseIntegrationTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private CompatibilityController _compatibilityController;

        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            this._compatibilityController = new CompatibilityController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        /// <summary>
        /// Tests the GET /name request.
        /// </summary>
        [Test]
        public void TestGetName()
        {
            // Add the user.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                });
            });
            
            // Test getting the name.
            var nameResponse = _compatibilityController.GetName("15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225", null).Result;
            Assert.AreEqual("success", nameResponse.Result);
            Assert.AreEqual("Test Name", nameResponse.Name);
            nameResponse = _compatibilityController.GetName(null, "123456789").Result;
            Assert.AreEqual("success", nameResponse.Result);
            Assert.AreEqual("Test Name", nameResponse.Name);
            nameResponse = _compatibilityController.GetName("123456789", null).Result;
            Assert.AreEqual("success", nameResponse.Result);
            Assert.IsNull(nameResponse.Name);
        }
        
        /// <summary>
        /// Tests the GET /userbalance request.
        /// </summary>
        [Test]
        public void TestUserBalance()
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
                
                // Add the users.
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d",
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
            
            // Test getting the user balance.
            var userBalanceResponse = _compatibilityController.GetUserBalance("15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225", null).Result;
            Assert.AreEqual("success", userBalanceResponse.Result);
            Assert.IsTrue(userBalanceResponse.Balance.HasValue);
            Assert.That(Math.Abs(userBalanceResponse.Balance.Value - 0) < 0.001);
            userBalanceResponse = _compatibilityController.GetUserBalance("42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d", null).Result;
            Assert.AreEqual("success", userBalanceResponse.Result);
            Assert.IsTrue(userBalanceResponse.Balance.HasValue);
            Assert.That(Math.Abs(userBalanceResponse.Balance.Value - 0.5) < 0.001);
            userBalanceResponse = _compatibilityController.GetUserBalance(null, "123456789").Result;
            Assert.AreEqual("success", userBalanceResponse.Result);
            Assert.IsTrue(userBalanceResponse.Balance.HasValue);
            Assert.That(Math.Abs(userBalanceResponse.Balance.Value - 0) < 0.001);
            userBalanceResponse = _compatibilityController.GetUserBalance(null, "123456780").Result;
            Assert.AreEqual("success", userBalanceResponse.Result);
            Assert.IsTrue(userBalanceResponse.Balance.HasValue);
            Assert.That(Math.Abs(userBalanceResponse.Balance.Value - 0.5) < 0.001);
            userBalanceResponse = _compatibilityController.GetUserBalance("unknown_hash", null).Result;
            Assert.AreEqual("success", userBalanceResponse.Result);
            Assert.IsFalse(userBalanceResponse.Balance.HasValue);
        }

        /// <summary>
        /// Tests the /appenduser endpoint.
        /// </summary>
        [Test]
        public void TestAppendUser()
        {
            // Add 2 users.
            var userResponse = _compatibilityController.AppendUser("15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225", null, "Test User", "test1@email", "Test College", "Test User").Result;
            Assert.AreEqual("success", userResponse.Status);
            userResponse = _compatibilityController.AppendUser(null, "123456780", "Test User", "test2@email", "Test College", "Test User").Result;
            Assert.AreEqual("success", userResponse.Status);
            
            // Check that the users were created.
            using var context = new ConstructContext();
            Assert.AreEqual("test1@email", context.Users.First(u => u.HashedId == "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225").Email);
            Assert.AreEqual("test2@email", context.Users.First(u => u.HashedId == "42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d").Email);
        }
        
        /// <summary>
        /// Tests the /appendswipelog endpoint.
        /// </summary>
        [Test]
        public void TestAppendSwipeLog()
        {
            // Add the user.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                });
            });
            
            // Add the swipe logs and check they were added.
            using var context = new ConstructContext();
            var userResponse = _compatibilityController.AppendSwipeLog("15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225", null).Result;
            Assert.AreEqual("success", userResponse.Status);
            Assert.AreEqual(1, context.VisitLogs.Include(c => c.User)
                .Where(v => v.User.HashedId == "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225" && v.Source == "MainLab").ToList().Count);
            userResponse = _compatibilityController.AppendSwipeLog(null, "123456789").Result;
            Assert.AreEqual("success", userResponse.Status);
            Assert.AreEqual(2, context.VisitLogs.Include(c => c.User)
                .Where(v => v.User.HashedId == "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225" && v.Source == "MainLab").ToList().Count);
        }
    }
}