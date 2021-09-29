using System;
using System.Linq;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Database.Context;
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
        /// Add request for the swipe controller.
        /// </summary>
        private AddRequest _addRequest;

        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            this._addRequest = new AddRequest()
            {
                HashedId = "test_hash",
                Source = "test_source",
            };
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
            
            // Run a request and make sure it added the swipe log.
            using var context = new ConstructContext();
            var response = this._swipeController.Add(this._addRequest).Result.Value;
            Assert.AreEqual(200, this._swipeController.Response.StatusCode);
            Assert.AreEqual("success", response.Status);
            Assert.AreEqual(1, context.VisitLogs.ToList().Count);
            
            // Re-run a request and make sure it added the swipe log.
            response = this._swipeController.Add(this._addRequest).Result.Value;
            Assert.AreEqual(200, this._swipeController.Response.StatusCode);
            Assert.AreEqual("success", response.Status);
            Assert.AreEqual(2, context.VisitLogs.ToList().Count);
        }

        /// <summary>
        /// Tests the /swipe/add endpoint with a null HashedId.
        /// </summary>
        [Test]
        public void TestAddNullHashedId()
        {
            this._addRequest.HashedId = null;
            var response = this._swipeController.Add(this._addRequest).Result.Value;
            Assert.AreEqual(400, this._swipeController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }
        
        /// <summary>
        /// Tests the /swipe/add endpoint with an empty HashedId.
        /// </summary>
        [Test]
        public void TestAddEmptyHashedId()
        {
            this._addRequest.HashedId = "";
            var response = this._swipeController.Add(this._addRequest).Result.Value;
            Assert.AreEqual(400, this._swipeController.Response.StatusCode);
            Assert.AreEqual("missing-hashed-id", response.Status);
        }

        /// <summary>
        /// Tests the /swipe/add endpoint with a null Source.
        /// </summary>
        [Test]
        public void TestAddNullSource()
        {
            this._addRequest.Source = null;
            var response = this._swipeController.Add(this._addRequest).Result.Value;
            Assert.AreEqual(400, this._swipeController.Response.StatusCode);
            Assert.AreEqual("missing-source", response.Status);
        }
        
        /// <summary>
        /// Tests the /swipe/add endpoint with an empty Source.
        /// </summary>
        [Test]
        public void TestAddEmptySource()
        {
            this._addRequest.Source = "";
            var response = this._swipeController.Add(this._addRequest).Result.Value;
            Assert.AreEqual(400, this._swipeController.Response.StatusCode);
            Assert.AreEqual("missing-source", response.Status);
        }
        
        /// <summary>
        /// Tests the /swipe/add endpoint with an unregistered user.
        /// </summary>
        [Test]
        public void TestAddUnregisteredUser()
        {
            var response = this._swipeController.Add(this._addRequest).Result.Value;
            Assert.AreEqual(404, this._swipeController.Response.StatusCode);
            Assert.AreEqual("user-not-found", response.Status);
        }
    }
}