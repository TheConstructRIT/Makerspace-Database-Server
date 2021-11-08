using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.Print.Controllers;
using Construct.Print.Data.Request;
using Construct.Print.Data.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        /// Request used to create a print.
        /// </summary>
        private AddPrintRequest _addPrintRequest;

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
            
            // Set up the test request.
            this._addPrintRequest = new AddPrintRequest()
            {
                HashedId = "test_hash_2",
                FileName = "Test Name 3",
                Material = "Test1",
                Weight = 10,
                Purpose = "Test Purpose 3",
                BillTo = "Test Bill To 3",
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
            Assert.AreEqual(0.3f, response.Weight);
            Assert.AreEqual("Test Purpose 2", response.Purpose);
            Assert.AreEqual("Test Bill To 2", response.BillTo);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with a null hashed id.
        /// </summary>
        [Test]
        public void TestPostAddPrintNullHashedId()
        {
            this._addPrintRequest.HashedId = null;
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with an empty hashed id.
        /// </summary>
        [Test]
        public void TestPostAddPrintEmptyHashedId()
        {
            this._addPrintRequest.HashedId = "";
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with a null file name.
        /// </summary>
        [Test]
        public void TestPostAddPrintNullFileName()
        {
            this._addPrintRequest.FileName = null;
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-file-name", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with an empty file name.
        /// </summary>
        [Test]
        public void TestPostAddPrintEmptyFileName()
        {
            this._addPrintRequest.FileName = "";
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-file-name", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with a null material.
        /// </summary>
        [Test]
        public void TestPostAddPrintNullMaterial()
        {
            this._addPrintRequest.Material = null;
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-material", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with an empty file name.
        /// </summary>
        [Test]
        public void TestPostAddPrintEmptyMaterial()
        {
            this._addPrintRequest.Material = "";
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-material", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with a null purpose.
        /// </summary>
        [Test]
        public void TestPostAddPrintNullPurpose()
        {
            this._addPrintRequest.Purpose = null;
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-purpose", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with an empty purpose.
        /// </summary>
        [Test]
        public void TestPostAddPrintEmptyPurpose()
        {
            this._addPrintRequest.Purpose = "";
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("missing-purpose", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with a negative weight.
        /// </summary>
        [Test]
        public void TestPostAddPrintNegativeWeight()
        {
            this._addPrintRequest.Weight = -1;
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(400, this._printController.Response.StatusCode);
            Assert.AreEqual("negative-weight", response.Status);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with an unknown user.
        /// </summary>
        [Test]
        public void TestPostAddPrintUserNotFound()
        {
            this._addPrintRequest.HashedId = "unknown_hash";
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual(404, this._printController.Response.StatusCode);
            Assert.AreEqual("user-not-found", response.Status);
        }

        /// <summary>
        /// Tests the /print/add endpoint.
        /// </summary>
        [Test]
        public void TestPostAddPrint()
        {
            // Send the request.
            var response = this._printController.PostAdd(this._addPrintRequest).Result.Value;
            Assert.AreEqual("success", response.Status);
            
            // Check that the print was added.
            using var context = new ConstructContext();
            var newPrint = context.PrintLog.Include(printLog => printLog.Material)
                .Include(printLog => printLog.User).OrderByDescending(p => p.Time).First();
            Assert.AreEqual("test_hash_2", newPrint.User.HashedId);
            Assert.AreEqual("Test1", newPrint.Material.Name);
            Assert.AreEqual(10, newPrint.WeightGrams);
            Assert.AreEqual("Test Purpose 3", newPrint.Purpose);
            Assert.AreEqual("Test Bill To 3", newPrint.BillTo);
            Assert.That(Math.Abs(newPrint.Cost - 0.30f) < 0.001);
            Assert.IsTrue(newPrint.Owed);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with no BillTo.
        /// </summary>
        [Test]
        public void TestPostAddPrintNullBillTo()
        {
            // Send the request.
            this._addPrintRequest.BillTo = null;
            this._printController.PostAdd(this._addPrintRequest).Wait();
            
            // Check that the print was added.
            using var context = new ConstructContext();
            var newPrint = context.PrintLog.Include(printLog => printLog.Material)
                .Include(printLog => printLog.User).OrderByDescending(p => p.Time).First();
            Assert.AreEqual("test_hash_2", newPrint.User.HashedId);
            Assert.AreEqual("Test1", newPrint.Material.Name);
            Assert.AreEqual(10, newPrint.WeightGrams);
            Assert.AreEqual("Test Purpose 3", newPrint.Purpose);
            Assert.IsNull(newPrint.BillTo);
            Assert.That(Math.Abs(newPrint.Cost - 0.30f) < 0.001);
            Assert.IsTrue(newPrint.Owed);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with an owed print.
        /// </summary>
        [Test]
        public void TestPostAddPrintOwed()
        {
            // Send the request.
            this._addPrintRequest.Owed = true;
            this._printController.PostAdd(this._addPrintRequest).Wait();
            
            // Check that the print was added.
            using var context = new ConstructContext();
            var newPrint = context.PrintLog.Include(printLog => printLog.Material)
                .Include(printLog => printLog.User).OrderByDescending(p => p.Time).First();
            Assert.AreEqual("test_hash_2", newPrint.User.HashedId);
            Assert.AreEqual("Test1", newPrint.Material.Name);
            Assert.AreEqual(10, newPrint.WeightGrams);
            Assert.AreEqual("Test Purpose 3", newPrint.Purpose);
            Assert.AreEqual("Test Bill To 3", newPrint.BillTo);
            Assert.That(Math.Abs(newPrint.Cost - 0.30f) < 0.001);
            Assert.IsTrue(newPrint.Owed);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with a not owed print.
        /// </summary>
        [Test]
        public void TestPostAddPrintNotOwed()
        {
            // Send the request.
            this._addPrintRequest.Owed = false;
            this._printController.PostAdd(this._addPrintRequest).Wait();
            
            // Check that the print was added.
            using var context = new ConstructContext();
            var newPrint = context.PrintLog.Include(printLog => printLog.Material)
                .Include(printLog => printLog.User).OrderByDescending(p => p.Time).First();
            Assert.AreEqual("test_hash_2", newPrint.User.HashedId);
            Assert.AreEqual("Test1", newPrint.Material.Name);
            Assert.AreEqual(10, newPrint.WeightGrams);
            Assert.AreEqual("Test Purpose 3", newPrint.Purpose);
            Assert.AreEqual("Test Bill To 3", newPrint.BillTo);
            Assert.That(Math.Abs(newPrint.Cost - 0.30f) < 0.001);
            Assert.IsFalse(newPrint.Owed);
        }
        
        /// <summary>
        /// Tests the /print/add endpoint with an unknown material.
        /// </summary>
        [Test]
        public void TestPostAddPrintUnknownMaterial()
        {
            // Send the request.
            this._addPrintRequest.Material = "Test2";
            this._printController.PostAdd(this._addPrintRequest).Wait();
            
            // Check that the print was added.
            using var context = new ConstructContext();
            var newPrint = context.PrintLog.Include(printLog => printLog.Material)
                .Include(printLog => printLog.User).OrderByDescending(p => p.Time).First();
            Assert.AreEqual("test_hash_2", newPrint.User.HashedId);
            Assert.AreEqual("Test2", newPrint.Material.Name);
            Assert.AreEqual(10, newPrint.WeightGrams);
            Assert.AreEqual("Test Purpose 3", newPrint.Purpose);
            Assert.AreEqual("Test Bill To 3", newPrint.BillTo);
            Assert.AreEqual(0, newPrint.Cost);
            Assert.IsTrue(newPrint.Owed);
        }
    }
}