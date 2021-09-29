using System;
using System.Linq;
using System.Threading.Tasks;
using Construct.Core.Attribute;
using Construct.Core.Configuration;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.User.Data.Correction;
using Construct.User.Data.Request;
using Construct.User.Data.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.User.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// Fetches information about an existing user.
        /// </summary>
        /// <param name="hashedId">Hashed id of the user.</param>
        [HttpGet]
        [Path("/user/get")]
        public async Task<ActionResult<IResponse>> Get(string hashedId)
        {
            // Get the user.
            await using var context = new ConstructContext();
            var user = await context.Users.Include(user => user.PrintLogs).Include(user => user.Permissions)
                .FirstOrDefaultAsync(user => user.HashedId.ToLower() == hashedId.ToLower());
            
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
                Permissions = user.Permissions.Where(permission => permission.IsActive()).Select(permission => permission.Name).ToList(),
            };
        }
        
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">User information sent to register.</param>
        [HttpPost]
        [Path("/user/register")]
        public async Task<ActionResult<IResponse>> Register([FromBody] RegisterUserRequest request)
        {
            // Return an error if a field is invalid.
            var validationErrorResponse = request.GetValidationErrorResponse();
            if (validationErrorResponse != null)
            {
                Response.StatusCode = 400;
                return new ActionResult<IResponse>(validationErrorResponse);
            }
            
            // Correct the email and return if it is invalid.
            var emailCorrection = new EmailCorrection()
            {
                ValidEmails = ConstructConfiguration.Configuration.Email.ValidEmails,
                Corrections = ConstructConfiguration.Configuration.Email.EmailCorrections,
            };
            try
            {
                request.Email = emailCorrection.CorrectEmail(request.Email);
            }
            catch (FormatException)
            {
                Response.StatusCode = 400;
                return new GenericStatusResponse("invalid-email");
            }
            
            // Return if the user already exists.
            await using var context = new ConstructContext();
            if (await context.Users.FirstOrDefaultAsync(user => user.HashedId.ToLower() == request.HashedId.ToLower() || user.Email.ToLower() == request.Email) != null)
            {
                Response.StatusCode = 409;
                return new GenericStatusResponse("duplicate-user");
            }
            
            // Add the user and return success.
            var user = new Core.Database.Model.User()
            {
                HashedId = request.HashedId.ToLower(),
                Name = request.Name,
                Email = request.Email,
                SignUpTime = DateTime.Now,
            };
            var student = new Student()
            {
                User = user,
                College = request.College,
                Year = request.Year,
            };
            context.Users.Add(user);
            context.Students.Add(student);
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }
    }
}