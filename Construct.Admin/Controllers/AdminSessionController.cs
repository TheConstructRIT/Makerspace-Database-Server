using System.Linq;
using System.Threading.Tasks;
using Construct.Admin.Data.Response;
using Construct.Admin.State;
using Construct.Core.Attribute;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.Admin.Controllers
{
    public class AdminSessionController : Controller
    {
        /// <summary>
        /// Creates a session for the given user.
        /// </summary>
        /// <param name="hashedId">Hashed id of the user.</param>
        [HttpGet]
        [Path("/admin/authenticate")]
        public async Task<ActionResult<IResponse>> GetAuthenticate(string hashedId)
        {
            // Return if no hashed id is provided.
            if (hashedId == null)
            {
                Response.StatusCode = 400;
                return new GenericStatusResponse("missing-hashed-id");
            }
            
            // Get the user.
            await using var context = new ConstructContext();
            var user = await context.Users.Include(user => user.Permissions)
                .FirstOrDefaultAsync(user => user.HashedId.ToLower() == hashedId.ToLower());
            
            // Return not found if the user isn't authorized.
            if (user?.Permissions.FirstOrDefault(p => p.Name.ToLower() == "labmanager") == null)
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }
            
            // Create and return the session.
            return new SessionResponse()
            {
                Session = Session.GetSingleton().CreateSession(hashedId)
            };
        }
        
        /// <summary>
        /// Checks if a session is valid.
        /// </summary>
        /// <param name="session">Session to check.</param>
        [HttpGet]
        [Path("/admin/checksession")]
        public ActionResult<IResponse> GetCheckSession(string session)
        {
            // Return if no session is provided.
            if (session == null)
            {
                Response.StatusCode = 400;
                return new GenericStatusResponse("missing-session");
            }
            
            // Return depending on if the session is valid.
            if (!Session.GetSingleton().SessionValid(session))
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }
            return new BaseSuccessResponse();
        }
    }
}