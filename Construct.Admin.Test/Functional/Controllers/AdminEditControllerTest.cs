using System;
using System.Linq;
using Construct.Admin.Controllers;
using Construct.Admin.Data.Request;
using Construct.Admin.State;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Construct.Admin.Test.Functional.Controllers
{
    public class AdminEditControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private AdminEditController _adminEditController;

        /// <summary>
        /// Session to use with the tests.
        /// </summary>
        private string _session = Session.GetSingleton().CreateSession("test");
        
        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            // Create the controller.
            this._adminEditController = new AdminEditController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            
            // Add the test user and print.
            this.AddData((context) =>
            {
                // Create the test user, material, and print.
                var testUser = new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                };
                var testMaterial = new PrintMaterial()
                {
                    Name = "TestMaterial",
                    CostPerGram = 0.03f,
                };
                var testPrint = new PrintLog()
                {
                    Key = 1,
                    User = testUser,
                    Time = DateTime.Now,
                    FileName = "TestPrint",
                    Material = testMaterial,
                    WeightGrams = 5,
                    Purpose = "TestPurpose",
                    BillTo = "TestBillTo",
                    Cost = 0.15f,
                    Owed = true,
                };
                
                // Add the test data.
                context.Users.Add(testUser);
                context.PrintMaterials.Add(testMaterial);
                context.PrintLog.Add(testPrint);
            });
        }

        /// <summary>
        /// Tests PostChangePrint with an unauthorized session.
        /// </summary>
        [Test]
        public void TestPostChangePrintUnauthorized()
        {
            Assert.AreEqual("unauthorized", this._adminEditController.PostChangePrint(new ChangePrintRequest()
            {
                Session = "unknown",
            }).Result.Value.Status);
            Assert.AreEqual(this._adminEditController.Response.StatusCode, 401);
        }
        
        /// <summary>
        /// Tests PostChangePrint with a missing filename.
        /// </summary>
        [Test]
        public void TestPostChangePrintMissingFileName()
        {
            Assert.AreEqual("missing-filename", this._adminEditController.PostChangePrint(new ChangePrintRequest()
            {
                Session = this._session,
                Id = 0,
                Purpose = "NewPurpose",
                Weight = 6,
                MSDNumber = null,
                Owed = false,
            }).Result.Value.Status);
            Assert.AreEqual(this._adminEditController.Response.StatusCode, 400);
        }
        
        /// <summary>
        /// Tests PostChangePrint with a missing purpose.
        /// </summary>
        [Test]
        public void TestPostChangePrintMissingPurpose()
        {
            Assert.AreEqual("missing-purpose", this._adminEditController.PostChangePrint(new ChangePrintRequest()
            {
                Session = this._session,
                Id = 0,
                FileName = "NewFileName",
                Weight = 6,
                MSDNumber = null,
                Owed = false,
            }).Result.Value.Status);
            Assert.AreEqual(this._adminEditController.Response.StatusCode, 400);
        }
        
        /// <summary>
        /// Tests PostChangePrint with a missing print.
        /// </summary>
        [Test]
        public void TestPostChangePrintNotFound()
        {
            Assert.AreEqual("print-not-found", this._adminEditController.PostChangePrint(new ChangePrintRequest()
            {
                Session = this._session,
                Id = 2,
                FileName = "NewFileName",
                Purpose = "NewPurpose",
                Weight = 6,
                MSDNumber = null,
                Owed = false,
            }).Result.Value.Status);
            Assert.AreEqual(this._adminEditController.Response.StatusCode, 404);
        }
        
        /// <summary>
        /// Tests PostChangePrint with no Bill To.
        /// </summary>
        [Test]
        public void TestPostChangePrintNoBillTo()
        {
            // Send the change request.
            Assert.AreEqual("success", this._adminEditController.PostChangePrint(new ChangePrintRequest()
            {
                Session = this._session,
                Id = 1,
                FileName = "NewFileName",
                Purpose = "NewPurpose",
                Weight = 6,
                MSDNumber = null,
                Owed = false,
            }).Result.Value.Status);
            
            // Assert the print is changed.
            using var context = new ConstructContext();
            var print = context.PrintLog.First();
            Assert.AreEqual(print.Key, 1);
            Assert.AreEqual(print.FileName, "NewFileName");
            Assert.AreEqual(print.Purpose, "NewPurpose");
            Assert.AreEqual(print.WeightGrams, 6);
            Assert.IsNull(print.BillTo);
            Assert.IsFalse(print.Owed);
        }
        
        /// <summary>
        /// Tests PostChangePrint with an empty Bill To.
        /// </summary>
        [Test]
        public void TestPostChangePrintEmptyBillTo()
        {
            // Send the change request.
            Assert.AreEqual("success", this._adminEditController.PostChangePrint(new ChangePrintRequest()
            {
                Session = this._session,
                Id = 1,
                FileName = "NewFileName",
                Purpose = "NewPurpose",
                Weight = 6,
                MSDNumber = "",
                Owed = false,
            }).Result.Value.Status);
            
            // Assert the print is changed.
            using var context = new ConstructContext();
            var print = context.PrintLog.First();
            Assert.AreEqual(print.Key, 1);
            Assert.AreEqual(print.FileName, "NewFileName");
            Assert.AreEqual(print.Purpose, "NewPurpose");
            Assert.AreEqual(print.WeightGrams, 6);
            Assert.IsNull(print.BillTo);
            Assert.IsFalse(print.Owed);
        }
        
        /// <summary>
        /// Tests PostChangePrint with a Bill To.
        /// </summary>
        [Test]
        public void TestPostChangePrintBillTo()
        {
            // Send the change request.
            Assert.AreEqual("success", this._adminEditController.PostChangePrint(new ChangePrintRequest()
            {
                Session = this._session,
                Id = 1,
                FileName = "NewFileName",
                Purpose = "NewPurpose",
                Weight = 6,
                MSDNumber = "TestBillTo",
                Owed = false,
            }).Result.Value.Status);
            
            // Assert the print is changed.
            using var context = new ConstructContext();
            var print = context.PrintLog.First();
            Assert.AreEqual(print.Key, 1);
            Assert.AreEqual(print.FileName, "NewFileName");
            Assert.AreEqual(print.Purpose, "NewPurpose");
            Assert.AreEqual(print.WeightGrams, 6);
            Assert.AreEqual(print.BillTo, "TestBillTo");
            Assert.IsFalse(print.Owed);
        }
    }
}