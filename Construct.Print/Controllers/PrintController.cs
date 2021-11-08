using System;
using System.Linq;
using System.Threading.Tasks;
using Construct.Core.Attribute;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.Print.Data.Request;
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
        
        /// <summary>
        /// Adds a print to the system.
        /// </summary>
        [HttpPost]
        [Path("/print/add")]
        public async Task<ActionResult<IResponse>> PostAdd([FromBody] AddPrintRequest request)
        {
            // Return an error if a field is invalid.
            var validationErrorResponse = request.GetValidationErrorResponse();
            if (validationErrorResponse != null)
            {
                Response.StatusCode = 400;
                return new ActionResult<IResponse>(validationErrorResponse);
            }
            
            // Return an error if the weight is negative.
            if (request.Weight < 0)
            {
                Response.StatusCode = 400;
                return new GenericStatusResponse("negative-weight");
            }
            
            // Get the user.
            await using var context = new ConstructContext();
            var user = await context.Users.Include(user => user.PrintLogs)
                .FirstOrDefaultAsync(user => user.HashedId.ToLower() == request.HashedId.ToLower());
            
            // Return not found if the user doesn't exist.
            if (user == null)
            {
                Response.StatusCode = 404;
                return new GenericStatusResponse("user-not-found");
            }
            
            // Get the material and add a new material if it doesn't exist.
            var material = await context.PrintMaterials.FirstOrDefaultAsync(material => material.Name.ToLower() == request.Material.ToLower());
            if (material == null)
            {
                material = new PrintMaterial()
                {
                    Name = request.Material,
                    CostPerGram = 0,
                };
                context.PrintMaterials.Add(material);
            }
            
            // Add the print.
            user.PrintLogs.Add(new PrintLog()
            {
                User = user,
                Time = DateTime.Now,
                FileName = request.FileName,
                Material = material,
                WeightGrams = request.Weight,
                Purpose = request.Purpose,
                BillTo = request.BillTo,
                Cost = material.CostPerGram * request.Weight,
                Owed = request.Owed ?? true,
            });
            
            // Save the changes and return success.
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }
    }
}