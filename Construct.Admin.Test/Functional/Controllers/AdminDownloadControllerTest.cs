using Construct.Admin.Controllers;
using Construct.Admin.State;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Data.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Construct.Admin.Test.Functional.Controllers
{
    public class AdminDownloadControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private AdminDownloadController _adminDownloadController;

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
            this._adminDownloadController = new AdminDownloadController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            
            // Add the test user and print.
            this.AddData((context) =>
            {
                // TODO
            });
        }
        
        /// <summary>
        /// Tests GetCsvs with an unauthorized session.
        /// </summary>
        [Test]
        public void TestGetCsvsUnauthorized()
        {
            Assert.AreEqual("unauthorized", ((IResponse) this._adminDownloadController.GetCsvs("unknwon").Result.Value).Status);
            Assert.AreEqual(this._adminDownloadController.Response.StatusCode, 401);
        }
    }
}