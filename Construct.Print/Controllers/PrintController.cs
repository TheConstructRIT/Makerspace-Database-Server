using System;
using System.Linq;
using System.Threading.Tasks;
using Construct.Core.Attribute;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Print.Data.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.Print.Controllers
{
    public class PrintController : Controller
    {
        /// <summary>
        /// Fetches information about the last print of a user.
        /// </summary>
        /// <param name="hashedId">Hashed id of the user.</param>
        [HttpGet]
        [Path("/print/last")]
        public async Task<ActionResult<IResponse>> GetLast(string hashedId)
        {
            // Return if the hashed id is null or empty.
            if (string.IsNullOrEmpty(hashedId))
            {
                Response.StatusCode = 400;
                return new GenericStatusResponse("missing-hashed-id");
            }
            
            // Get the user.
            await using var context = new ConstructContext();
            var user = await context.Users.Include(user => user.PrintLogs)
                .FirstOrDefaultAsync(user => user.HashedId.ToLower() == hashedId.ToLower());
            
            // Return not found if the user doesn't exist.
            if (user == null)
            {
                Response.StatusCode = 404;
                return new GenericStatusResponse("user-not-found");
            }
            
            // Return if the user has no prints.
            if (user.PrintLogs.Count == 0)
            {
                Response.StatusCode = 404;
                return new GenericStatusResponse("no-prints");
            }
            
            // Return the response for the last print.
            var lastPrint = user.PrintLogs.OrderByDescending(p => p.Time).First();
            return new LastPrintResponse()
            {
                FileName = lastPrint.FileName,
                TimeStamp = ((DateTimeOffset) lastPrint.Time).ToUnixTimeSeconds(),
                Purpose = lastPrint.Purpose,
                BillTo = lastPrint.BillTo,
            };
        }
        
        // TODO: Log print
    }
}