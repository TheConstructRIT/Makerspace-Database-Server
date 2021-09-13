using System;
using System.Linq;
using System.Threading.Tasks;
using Construct.Core.Attribute;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.User.Data.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.User.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// Test request handler to be replaced later.
        /// </summary>
        /// <param name="hashedId">Hashed id of the user.</param>
        [HttpGet]
        [Path("/user/get")]
        public async Task<ActionResult<IResponse>> Get(string hashedId)
        {
            // Get the user.
            await using var context = new ConstructContext();
            var user = await context.Users.Include(user => user.PrintLogs)
                .FirstOrDefaultAsync(user => user.HashedId == hashedId);
            
            // Return not found if the user doesn't exist.
            if (user == null)
            {
                Response.StatusCode = 404;
                return new NotFoundResponse();
            }
            
            // Determine the balance for the print logs.
            var owedPrints = user.PrintLogs.Where(print => print.Owed);
            var owedPrintBalance = 0.0;
            foreach (var print in owedPrints)
            {
                owedPrintBalance += print.Cost;
            }
            
            // Return the response.
            return new GetUserResponse()
            {
                Name = user.Name,
                Email = user.Email,
                OwedPrintBalance = owedPrintBalance,
            };
        }
        
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
                SignUpTime = DateTime.Now,
            });
            context.SaveChanges();
            return context.Users.First().Email;
        }
    }
}