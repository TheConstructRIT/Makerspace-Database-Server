using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Core.Attribute;
using Construct.Core.Database.Context;
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
            using var context = new ConstructContext();
            context.Users.Add(new Core.Database.Model.User()
            {
                HashedId = "test",
                Name = "John Doe",
                Email = "test@email.com",
                Permissions = new List<string>() { "Test" },
                SignUpTime = DateTime.Now,
            });
            context.SaveChanges();
            return context.Users.First().Email;
        }
    }
}