using Construct.Core.Attribute;
using Microsoft.AspNetCore.Mvc;

namespace Construct.User.Controller
{
    public class TestController
    {
        /// <summary>
        /// Test request handler to be replaced later.
        /// </summary>
        [HttpGet]
        [Path("/user/test")]
        public ActionResult<string> Test()
        {
            return "Test response.";
        }
    }
}