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
                            Purpose = "Test Purpose",
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
                            Purpose = "Test Purpose",
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
                            Purpose = "Test Purpose",
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
        /// Tests the GET /isauthorized request.
        /// </summary>
        [Test]
        public void TestIsAuthorized()
        {
            // Add the users.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                    Permissions = new List<Permission>(),
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d",
                    Name = "Test Name",
                    Email = "test@email",
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
            
            // Test getting the authorized users.
            var authorizedResponse = _compatibilityController.GetIsAuthorized("15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225", null).Result;
            Assert.AreEqual("success", authorizedResponse.Result);
            Assert.IsFalse(authorizedResponse.Authorized);
            authorizedResponse = _compatibilityController.GetIsAuthorized("42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d", null).Result;
            Assert.AreEqual("success", authorizedResponse.Result);
            Assert.IsTrue(authorizedResponse.Authorized);
            authorizedResponse = _compatibilityController.GetIsAuthorized(null, "123456789").Result;
            Assert.AreEqual("success", authorizedResponse.Result);
            Assert.IsFalse(authorizedResponse.Authorized);
            authorizedResponse = _compatibilityController.GetIsAuthorized(null, "123456780").Result;
            Assert.AreEqual("success", authorizedResponse.Result);
            Assert.IsTrue(authorizedResponse.Authorized);
            authorizedResponse = _compatibilityController.GetIsAuthorized("unknown_hash", null).Result;
            Assert.AreEqual("success", authorizedResponse.Result);
            Assert.IsFalse(authorizedResponse.Authorized);
        }
        
        /// <summary>
        /// Tests the GET /hashedid request.
        /// </summary>
        [Test]
        public void TestHashedId()
        {
            // Add the users.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225",
                    Name = "Test Name",
                    Email = "test1@email",
                    SignUpTime = DateTime.Now,
                    Permissions = new List<Permission>(),
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d",
                    Name = "Test Name",
                    Email = "test2@email",
                    SignUpTime = DateTime.Now,
                });
            });
            
            // Test getting the authorized users.
            var hashedIdResponse = _compatibilityController.GetHashedId("test1@email").Result;
            Assert.AreEqual("success", hashedIdResponse.Result);
            Assert.AreEqual("15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225", hashedIdResponse.HashedId);
            hashedIdResponse = _compatibilityController.GetHashedId("test2@email").Result;
            Assert.AreEqual("success", hashedIdResponse.Result);
            Assert.AreEqual("42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d", hashedIdResponse.HashedId);
            hashedIdResponse = _compatibilityController.GetHashedId("test3@email").Result;
            Assert.AreEqual("success", hashedIdResponse.Result);
            Assert.IsNull(hashedIdResponse.HashedId);
            hashedIdResponse = _compatibilityController.GetHashedId("not_an_email").Result;
            Assert.AreEqual("success", hashedIdResponse.Result);
            Assert.IsNull(hashedIdResponse.HashedId);
        }

        /// <summary>
        /// Tests the GET /uesrinfo request.
        /// </summary>
        [Test]
        public void TestUserInfo()
        {
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
                    Email = "test1@email",
                    SignUpTime = DateTime.Now,
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d",
                    Name = "Test Name",
                    Email = "test2@email",
                    SignUpTime = DateTime.Now,
                    PrintLogs = new List<PrintLog>()
                    {
                        new PrintLog()
                        {
                            Time = DateTime.Now - new TimeSpan(30),
                            FileName = "Test File",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Purpose 1",
                            BillTo = "Test Bill To 1",
                            Cost = 0.3f,
                            Owed = true,
                        },
                        new PrintLog()
                        {
                            Time = DateTime.Now - new TimeSpan(20),
                            FileName = "Test File",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Purpose 2",
                            BillTo = "Test Bill To 2",
                            Cost = 0.6f,
                            Owed = false,
                        },
                        new PrintLog()
                        {
                            Time = DateTime.Now - new TimeSpan(10),
                            FileName = "Test File",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Purpose 3",
                            BillTo = "Test Bill To 3",
                            Cost = 0.2f,
                            Owed = true,
                        },
                    }
                });
            });
            
            // Test getting the user info.
            var userInfoResponse = _compatibilityController.GetUserInfo("15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225", null).Result;
            Assert.AreEqual("success", userInfoResponse.Result);
            Assert.AreEqual("test1@email", userInfoResponse.Email);
            Assert.IsNull(userInfoResponse.LastPurpose);
            Assert.IsNull(userInfoResponse.LastMSDNumber);
            userInfoResponse = _compatibilityController.GetUserInfo("42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d", null).Result;
            Assert.AreEqual("success", userInfoResponse.Result);
            Assert.AreEqual("test2@email", userInfoResponse.Email);
            Assert.AreEqual("Test Purpose 3", userInfoResponse.LastPurpose);
            Assert.AreEqual("Test Bill To 3", userInfoResponse.LastMSDNumber);
            userInfoResponse = _compatibilityController.GetUserInfo(null, "123456789").Result;
            Assert.AreEqual("success", userInfoResponse.Result);
            Assert.AreEqual("test1@email", userInfoResponse.Email);
            Assert.IsNull(userInfoResponse.LastPurpose);
            Assert.IsNull(userInfoResponse.LastMSDNumber);
            userInfoResponse = _compatibilityController.GetUserInfo(null, "123456780").Result;
            Assert.AreEqual("success", userInfoResponse.Result);
            Assert.AreEqual("test2@email", userInfoResponse.Email);
            Assert.AreEqual("Test Purpose 3", userInfoResponse.LastPurpose);
            Assert.AreEqual("Test Bill To 3", userInfoResponse.LastMSDNumber);
            userInfoResponse = _compatibilityController.GetUserInfo("unknown_hash", null).Result;
            Assert.AreEqual("success", userInfoResponse.Result);
            Assert.IsNull(userInfoResponse.Email);
            Assert.IsNull(userInfoResponse.LastPurpose);
            Assert.IsNull(userInfoResponse.LastMSDNumber);
        }
        
        /// <summary>
        /// Tests the GET /lastprinttime request.
        /// </summary>
        [Test]
        public void TestLastPrintTime()
        {
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
                    Email = "test1@email",
                    SignUpTime = DateTime.Now,
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d",
                    Name = "Test Name",
                    Email = "test2@email",
                    SignUpTime = DateTime.Now,
                    PrintLogs = new List<PrintLog>()
                    {
                        new PrintLog()
                        {
                            Time = DateTimeOffset.FromUnixTimeSeconds(1633546210).LocalDateTime,
                            FileName = "Test File",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Purpose 1",
                            BillTo = "Test Bill To 1",
                            Cost = 0.3f,
                            Owed = true,
                        },
                        new PrintLog()
                        {
                            Time = DateTimeOffset.FromUnixTimeSeconds(1633546220).LocalDateTime,
                            FileName = "Test File",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Purpose 2",
                            BillTo = "Test Bill To 2",
                            Cost = 0.6f,
                            Owed = false,
                        },
                        new PrintLog()
                        {
                            Time = DateTimeOffset.FromUnixTimeSeconds(1633546230).LocalDateTime,
                            FileName = "Test File",
                            Material = testMaterial,
                            WeightGrams = 0.3f,
                            Purpose = "Test Purpose 3",
                            BillTo = "Test Bill To 3",
                            Cost = 0.2f,
                            Owed = true,
                        },
                    }
                });
            });
            
            // Test getting the user info.
            var lastPrintTimeResponse = _compatibilityController.GetLastPrintTime("15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225", null).Result;
            Assert.AreEqual("success", lastPrintTimeResponse.Result);
            Assert.IsFalse(lastPrintTimeResponse.LastPrintTime.HasValue);
            Assert.IsFalse(lastPrintTimeResponse.LastPrintWeight.HasValue);
            lastPrintTimeResponse = _compatibilityController.GetLastPrintTime("42dd0a7fdcb47aad0f6bd98da39c42ba60c00dc0e01fcba36195c23b7f19143d", null).Result;
            Assert.AreEqual("success", lastPrintTimeResponse.Result);
            Assert.AreEqual(1633546230, lastPrintTimeResponse.LastPrintTime.Value);
            Assert.That(Math.Abs(lastPrintTimeResponse.LastPrintWeight.Value - 0.3f) < 0.001);
            lastPrintTimeResponse = _compatibilityController.GetLastPrintTime(null, "123456789").Result;
            Assert.AreEqual("success", lastPrintTimeResponse.Result);
            Assert.IsFalse(lastPrintTimeResponse.LastPrintTime.HasValue);
            Assert.IsFalse(lastPrintTimeResponse.LastPrintWeight.HasValue);
            lastPrintTimeResponse = _compatibilityController.GetLastPrintTime(null, "123456780").Result;
            Assert.AreEqual("success", lastPrintTimeResponse.Result);
            Assert.AreEqual(1633546230, lastPrintTimeResponse.LastPrintTime.Value);
            Assert.That(Math.Abs(lastPrintTimeResponse.LastPrintWeight.Value - 0.3f) < 0.001);
            lastPrintTimeResponse = _compatibilityController.GetLastPrintTime("unknown_hash", null).Result;
            Assert.AreEqual("success", lastPrintTimeResponse.Result);
            Assert.IsFalse(lastPrintTimeResponse.LastPrintTime.HasValue);
            Assert.IsFalse(lastPrintTimeResponse.LastPrintWeight.HasValue);
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

        /// <summary>
        /// Tests the /appendprint endpoint with no user.
        /// </summary>
        [Test]
        public void TestAppendPrintNoUser()
        {
            this._compatibilityController.AppendPrint("test@email", "testfile", "PLA", 4, null, "Test Purpose", null, null);
            using var context = new ConstructContext();
            Assert.AreEqual(0, context.PrintLog.ToList().Count);
        }
        
        /// <summary>
        /// Tests the /appendprint endpoint with no material.
        /// </summary>
        [Test]
        public void TestAppendPrintNoMaterial()
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
            
            // Test that the print was not added.
            this._compatibilityController.AppendPrint("test@email", "testfile", "PLA", 4, null, "Test Purpose", null, null);
            using var context = new ConstructContext();
            Assert.AreEqual(0, context.PrintLog.ToList().Count);
        }
        
        /// <summary>
        /// Tests the /appendprint endpoint with no MSD Number.
        /// </summary>
        [Test]
        public void TestAppendPrintNoMsd()
        {
            // Add the material and user.
            this.AddData((context) =>
            {
                context.PrintMaterials.Add(new PrintMaterial()
                {
                    Name = "PLA",
                    CostPerGram = 0.03f,
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                });
            });
            
            // Test that the print was not added.
            this._compatibilityController.AppendPrint("test@email", "testfile", "PLA", 4, null, "Test Purpose", null, null);
            using var context = new ConstructContext();
            var print = context.PrintLog.Include(p => p.Material).Include(p => p.User).First();
            Assert.AreEqual("test@email", print.User.Email);
            Assert.AreEqual("testfile", print.FileName);
            Assert.AreEqual("PLA", print.Material.Name);
            Assert.That(Math.Abs(print.WeightGrams - 4f) < 0.001);
            Assert.AreEqual("Test Purpose", print.Purpose);
            Assert.IsNull(print.BillTo);
            Assert.That(Math.Abs(print.Cost - 0.12f) < 0.001);
            Assert.IsTrue(print.Owed);
        }
        
        /// <summary>
        /// Tests the /appendprint endpoint with an MSD Number.
        /// </summary>
        [Test]
        public void TestAppendPrintMsd()
        {
            // Add the material and user.
            this.AddData((context) =>
            {
                context.PrintMaterials.Add(new PrintMaterial()
                {
                    Name = "PLA",
                    CostPerGram = 0.03f,
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                });
            });
            
            // Test that the print was not added.
            this._compatibilityController.AppendPrint("test@email", "testfile", "PLA", 4, null, "Test Purpose", "P12345", null);
            using var context = new ConstructContext();
            var print = context.PrintLog.Include(p => p.Material).Include(p => p.User).First();
            Assert.AreEqual("test@email", print.User.Email);
            Assert.AreEqual("testfile", print.FileName);
            Assert.AreEqual("PLA", print.Material.Name);
            Assert.That(Math.Abs(print.WeightGrams - 4f) < 0.001);
            Assert.AreEqual("Test Purpose", print.Purpose);
            Assert.AreEqual("P12345", print.BillTo);
            Assert.That(Math.Abs(print.Cost - 0.12f) < 0.001);
            Assert.IsTrue(print.Owed);
        }
        
        /// <summary>
        /// Tests the /appendprint endpoint with the payment not owed.
        /// </summary>
        [Test]
        public void TestAppendPrintNotOwed()
        {
            // Add the material and user.
            this.AddData((context) =>
            {
                context.PrintMaterials.Add(new PrintMaterial()
                {
                    Name = "PLA",
                    CostPerGram = 0.03f,
                });
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "15e2b0d3c33891ebb0f1ef609ec419420c20e320ce94c65fbc8c3312448eb225",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                });
            });
            
            // Test that the print was not added.
            this._compatibilityController.AppendPrint("test@email", "testfile", "PLA", 4, null, "Test Purpose", null, false);
            using var context = new ConstructContext();
            var print = context.PrintLog.Include(p => p.Material).Include(p => p.User).First();
            Assert.AreEqual("test@email", print.User.Email);
            Assert.AreEqual("testfile", print.FileName);
            Assert.AreEqual("PLA", print.Material.Name);
            Assert.That(Math.Abs(print.WeightGrams - 4f) < 0.001);
            Assert.AreEqual("Test Purpose", print.Purpose);
            Assert.IsNull(print.BillTo);
            Assert.That(Math.Abs(print.Cost - 0.12f) < 0.001);
            Assert.IsFalse(print.Owed);
        }
    }
}