using Construct.Core.Data.Response;
using Construct.Core.Test.Integration.Base;
using Construct.User.Controllers;
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
    }
}