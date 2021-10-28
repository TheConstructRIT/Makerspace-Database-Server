using System.Threading.Tasks;
using Construct.Admin.Data.Request;
using Construct.Admin.State;
using Construct.Core.Attribute;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
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
            print.BillTo = (request.MSDNumber == "" ? null : request.MSDNumber);
            print.Owed = request.Owed;
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }
    }
}