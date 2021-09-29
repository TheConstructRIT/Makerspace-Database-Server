using System;
using System.Threading.Tasks;
using Construct.Core.Attribute;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.Swipe.Data.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.Swipe.Controllers
{
    public class SwipeController : Controller
    {
        /// <summary>
        /// Adds a swipe log to the database.
        /// </summary>
        /// <param name="request">Swipe information sent to add.</param>
        [HttpPost]
        [Path("/swipe/add")]
        public async Task<ActionResult<IResponse>> Add([FromBody] AddRequest request)
        {
            // Return an error if a field is invalid.
            var validationErrorResponse = request.GetValidationErrorResponse();
            if (validationErrorResponse != null)
            {
                Response.StatusCode = 400;
                return new ActionResult<IResponse>(validationErrorResponse);
            }
            
            // Return if the user doesn't exist.
            await using var context = new ConstructContext();
            var swipedUser = await context.Users.FirstOrDefaultAsync(user => user.HashedId.ToLower() == request.HashedId.ToLower());
            if (swipedUser == null)
            {
                Response.StatusCode = 404;
                return new GenericStatusResponse("user-not-found");
            }
            
            // Add the swipe log and return success.
            context.VisitLogs.Add(new VisitLog()
            {
                User = swipedUser,
                Time = DateTime.Now,
                Source = request.Source,
            });
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }
    }
}