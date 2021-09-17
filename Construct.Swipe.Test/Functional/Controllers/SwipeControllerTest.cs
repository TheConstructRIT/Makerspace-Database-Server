using System;
using System.Linq;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.Core.Test.Functional.Base;
using Construct.Swipe.Controllers;
using Construct.Swipe.Data.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Construct.Swipe.Test.Functional.Controllers
{
    public class SwipeControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private SwipeController _swipeController;

        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            this._swipeController = new SwipeController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }
        
        /// <summary>
        /// Tests the /swipe/add endpoint with a user.
        /// </summary>
        [Test]
        public void TestAdd()
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
            
            // Create the request.
            var request = new AddBody()
            {
                HashedId = "test_hash",
                Source = "test_source"
            };
            
            // Run a request and make sure it added the swipe log.
            using var context = new ConstructContext();
            var response = this._swipeController.Add(request).Result.Value;
            Assert.AreEqual(200, this._swipeController.Response.StatusCode);
            Assert.AreEqual("success", response.Status);
            Assert.AreEqual(1, context.VisitLogs.ToList().Count);
            
            // Re-run a request and make sure it added the swipe log.
            response = this._swipeController.Add(request).Result.Value;
            Assert.AreEqual(200, this._swipeController.Response.StatusCode);
            Assert.AreEqual("success", response.Status);
            Assert.AreEqual(2, context.VisitLogs.ToList().Count);
        }
        
        /// <summary>
        /// Tests the /swipe/add endpoint with missing or invalid data.
        /// </summary>
        [Test]
        public void TestAddErrors()
        {
            // Create the base response.
            var request = new AddBody()
            {
                HashedId = "test_hash",
                Source = "test_source"
            };
            
            // Test the HashedId field.
            request.HashedId = null;
            var response = this._swipeController.Add(request).Result.Value;
            Assert.AreEqual(400, this._swipeController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);

            request.HashedId = "";
            this._swipeController.Response.StatusCode = 200;
            response = this._swipeController.Add(request).Result.Value;
            Assert.AreEqual(400, this._swipeController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
            request.HashedId = "test_hash";
            
            // Test the Name field.
            request.Source = null;
            this._swipeController.Response.StatusCode = 200;
            response = this._swipeController.Add(request).Result.Value;
            Assert.AreEqual(400, this._swipeController.Response.StatusCode);
            Assert.AreEqual("missing-source", response.Status);

            request.Source = "";
            this._swipeController.Response.StatusCode = 200;
            response = this._swipeController.Add(request).Result.Value;
            Assert.AreEqual(400, this._swipeController.Response.StatusCode);
            Assert.AreEqual("missing-source", response.Status);
            request.Source = "test_source";
            
            // Test with the user not in the database.
            this._swipeController.Response.StatusCode = 200;
            response = this._swipeController.Add(request).Result.Value;
            Assert.AreEqual(404, this._swipeController.Response.StatusCode);
            Assert.AreEqual("user-not-found", response.Status);
        }
    }
}