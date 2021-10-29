using System.Linq;
using System.Threading.Tasks;
using Construct.Admin.Data.Request;
using Construct.Admin.State;
using Construct.Core.Attribute;
using Construct.Core.Configuration;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.Admin.Controllers
{
    public class AdminEditController : Controller
    {
        /// <summary>
        /// Changes a print in the database.
        /// </summary>
        /// <param name="request">Request data of the print to change.</param>
        [HttpPost]
        [Path("/admin/changeprint")]
        public async Task<ActionResult<IResponse>> PostChangePrint([FromBody] ChangePrintRequest request)
        {
            // Return if the session isn't valid.
            if (!Session.GetSingleton().RefreshSession(request.Session))
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }
            
            // Return an error if a field is invalid.
            var validationErrorResponse = request.GetValidationErrorResponse();
            if (validationErrorResponse != null)
            {
                Response.StatusCode = 400;
                return new ActionResult<IResponse>(validationErrorResponse);
            }
            
            // Get the print and return an error if none exists.
            await using var context = new ConstructContext();
            var print = await context.PrintLog.FirstOrDefaultAsync(printLog => printLog.Key == request.Id);
            if (print == null)
            {
                Response.StatusCode = 404;
                return new GenericStatusResponse("print-not-found");
            }
            
            // Update the print, save the print, and return success.
            print.FileName = request.FileName;
            print.Purpose = request.Purpose;
            print.WeightGrams = request.Weight;
            print.BillTo = (request.BillTo == "" ? null : request.BillTo);
            print.Owed = request.Owed;
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }
        
        /// <summary>
        /// Changes a user in the database.
        /// </summary>
        /// <param name="request">Request data of the user to change.</param>
        [HttpPost]
        [Path("/admin/changeuser")]
        public async Task<ActionResult<IResponse>> PostChangeUser([FromBody] ChangeUserRequest request)
        {
            // Return if the session isn't valid.
            if (!Session.GetSingleton().RefreshSession(request.Session))
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }
            
            // Return an error if a field is invalid.
            var validationErrorResponse = request.GetValidationErrorResponse();
            if (validationErrorResponse != null)
            {
                Response.StatusCode = 400;
                return new ActionResult<IResponse>(validationErrorResponse);
            }
            
            // Get the user and return an error if none exists.
            await using var context = new ConstructContext();
            var user = await context.Users.Include(user => user.Permissions)
                .FirstOrDefaultAsync(printLog => printLog.HashedId == request.HashedId);
            if (user == null)
            {
                Response.StatusCode = 404;
                return new GenericStatusResponse("user-not-found");
            }
            
            // Update the user.
            user.Name = request.Name;
            user.Email = request.Email;
            foreach (var permissionName in ConstructConfiguration.Configuration.Admin.ConfigurablePermissions)
            {
                if (request.Permissions.All(permissionPair => permissionPair.Key.ToLower() != permissionName.ToLower())) continue;
                var newPermission = request.Permissions.First(permissionPair => permissionPair.Key.ToLower() == permissionName.ToLower()).Value;
                var existingPermission = user.Permissions.FirstOrDefault(permission => permission.Name.ToLower() == permissionName.ToLower());
                if (newPermission)
                {
                    // Either make the permission no longer expired or add the permission.
                    if (existingPermission == null)
                    {
                        user.Permissions.Add(new Permission()
                        {
                            Name = permissionName,
                        });
                    }
                    else
                    {
                        existingPermission.StartTime = null;
                        existingPermission.EndTime = null;
                    }
                }
                else
                {
                    // Remove the permission if it is active.
                    if (existingPermission != null && existingPermission.IsActive())
                    {
                        user.Permissions.Remove(existingPermission);
                    }
                }
            }
            
            // Save the user and return success.
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }
        
        /// <summary>
        /// Clears the print balance of a user.
        /// </summary>
        /// <param name="request">Request data of the user to clear.</param>
        [HttpPost]
        [Path("/admin/clearbalance")]
        public async Task<ActionResult<IResponse>> PostClearBalance([FromBody] ClearBalanceRequest request)
        {
            // Return if the session isn't valid.
            if (!Session.GetSingleton().RefreshSession(request.Session))
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }
            
            // Return an error if a field is invalid.
            var validationErrorResponse = request.GetValidationErrorResponse();
            if (validationErrorResponse != null)
            {
                Response.StatusCode = 400;
                return new ActionResult<IResponse>(validationErrorResponse);
            }
            
            // Get the user and return an error if none exists.
            await using var context = new ConstructContext();
            var user = await context.Users.Include(user => user.PrintLogs).FirstOrDefaultAsync(printLog => printLog.HashedId == request.HashedId);
            if (user == null)
            {
                Response.StatusCode = 404;
                return new GenericStatusResponse("user-not-found");
            }
            
            // Clear the print balance, save the user, and return success.
            foreach (var print in user.PrintLogs)
            {
                print.Owed = false;
            }
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }
    }
}