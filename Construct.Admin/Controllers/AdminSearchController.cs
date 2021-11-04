using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Construct.Admin.Data.Response;
using Construct.Admin.State;
using Construct.Core.Attribute;
using Construct.Core.Configuration;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.Admin.Controllers
{
    public class AdminSearchController : Controller
    {
        /// <summary>
        /// Searches for prints in the database.
        /// </summary>
        /// <param name="session">Session of the user.</param>
        /// <param name="maxPrints">Maximum prints to display.</param>
        /// <param name="offsetPrints">Offset of the prints to return.</param>
        /// <param name="order">Column to sort by.</param>
        /// <param name="ascending">Whether to search by ascending.</param>
        /// <param name="search">String to search for.</param>
        /// <param name="hashedId">Hashed id to filter for.</param>
        /// <returns>The results of the search.</returns>
        [HttpGet]
        [Path("/admin/prints")]
        public async Task<ActionResult<IResponse>> GetPrints(string session, int maxPrints, int offsetPrints, string order, bool ascending = false, string search = "", string hashedId = null)
        {
            // Return if the session isn't valid.
            if (!Session.GetSingleton().RefreshSession(session))
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }
            
            // Build the base query.
            search = (search ?? "").ToLower();
            order = (order ?? "").ToLower() + (ascending ? "" : "Descending");
            await using var context = new ConstructContext();
            var basePrintsQuery = context.PrintLog.Include(printLog => printLog.User)
                .Include(printLog => printLog.Material)
                .Where(printLog => printLog.FileName.ToLower().Contains(search) || (printLog.BillTo != null && printLog.BillTo.ToLower().Contains(search)));
            if (hashedId != null)
            {
                basePrintsQuery = basePrintsQuery.Where(printLog => printLog.User != null && printLog.User.HashedId == hashedId);
            }

            // Add the ordering to the query.
            basePrintsQuery = order switch
            {
                "timeDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.Time),
                "time" => basePrintsQuery.OrderBy(printLog => printLog.Time),
                "filenameDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.FileName.ToLower()),
                "filename" => basePrintsQuery.OrderBy(printLog => printLog.FileName.ToLower()),
                "purposeDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.Purpose.ToLower()),
                "purpose" => basePrintsQuery.OrderBy(printLog => printLog.Purpose.ToLower()),
                "materialDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.Material.Name.ToLower()),
                "material" => basePrintsQuery.OrderBy(printLog => printLog.Material.Name.ToLower()),
                "weightDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.WeightGrams),
                "weight" => basePrintsQuery.OrderBy(printLog => printLog.WeightGrams),
                "costDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.Cost),
                "cost" => basePrintsQuery.OrderBy(printLog => printLog.Cost),
                "owedDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.Owed),
                "owed" => basePrintsQuery.OrderBy(printLog => printLog.Owed),
                "billtoDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.BillTo.ToLower()),
                "billto" => basePrintsQuery.OrderBy(printLog => printLog.BillTo.ToLower()),
                "userDescending" => basePrintsQuery.OrderByDescending(printLog => printLog.User.Email.ToLower()),
                "user" => basePrintsQuery.OrderBy(printLog => printLog.User.Email.ToLower()),
                _ => basePrintsQuery.OrderBy(printLog => printLog.FileName.ToLower()),
            };
            
            // Return the prints.
            var prints = new List<PrintResponseEntry>();
            foreach (var printLog in basePrintsQuery.Skip(offsetPrints).Take(maxPrints).ToList())
            {
                PrintResponseEntryUser user = null;
                if (printLog.User != null)
                {
                    user = new PrintResponseEntryUser()
                    {
                        Email = printLog.User.Email,
                        Name = printLog.User.Name,
                    };
                }
                prints.Add(new PrintResponseEntry()
                {
                    User = user,
                    Print = new PrintResponseEntryPrint()
                    {
                        Id = printLog.Key,
                        Name = printLog.FileName,
                        Timestamp = ((DateTimeOffset) printLog.Time).ToUnixTimeSeconds(),
                        Material = printLog.Material.Name,
                        Weight = printLog.WeightGrams,
                        Purpose = printLog.Purpose,
                        BillTo = printLog.BillTo,
                        Cost = printLog.Cost,
                        Owed = printLog.Owed,
                    },
                });
            }
            return new PrintsResponse()
            {
                TotalPrints = basePrintsQuery.Count(),
                Prints = prints,
            };
        }
        
        /// <summary>
        /// Searches for users in the database.
        /// </summary>
        /// <param name="session">Session of the user.</param>
        /// <param name="maxUsers">Maximum users to display.</param>
        /// <param name="offsetUsers">Offset of the users to return.</param>
        /// <param name="order">Column to sort by.</param>
        /// <param name="ascending">Whether to search by ascending.</param>
        /// <param name="search">String to search for.</param>
        /// <returns>The results of the search.</returns>
        [HttpGet]
        [Path("/admin/users")]
        public async Task<ActionResult<IResponse>> GetUsers(string session, int maxUsers, int offsetUsers, string order, bool ascending = false, string search = "")
        {
            // Return if the session isn't valid.
            if (!Session.GetSingleton().RefreshSession(session))
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }
            
            // Build the base query.
            search = (search ?? "").ToLower();
            order = (order ?? "").ToLower() + (ascending ? "" : "Descending");
            await using var context = new ConstructContext();
            var basePrintsQuery = context.Users.Include(user => user.PrintLogs).Include(user => user.Permissions)
                .Where(user => user.Name.ToLower().Contains(search) || user.Email.ToLower().Contains(search));

            // Add the ordering to the query.
            basePrintsQuery = order switch
            {
                "nameDescending" => basePrintsQuery.OrderByDescending(user => user.Name.ToLower()),
                "name" => basePrintsQuery.OrderBy(user => user.Name.ToLower()),
                "emailDescending" => basePrintsQuery.OrderByDescending(user => user.Email.ToLower()),
                "email" => basePrintsQuery.OrderBy(user => user.Email.ToLower()),
                "totalprintsDescending" => basePrintsQuery.OrderByDescending(user => user.PrintLogs.Count),
                "totalprints" => basePrintsQuery.OrderBy(user => user.PrintLogs.Count),
                "totalweightDescending" => basePrintsQuery.OrderByDescending(user => user.PrintLogs.Sum(printLog => printLog.WeightGrams)),
                "totalweight" => basePrintsQuery.OrderBy(user => user.PrintLogs.Sum(printLog => printLog.WeightGrams)),
                "totalowedprintsDescending" => basePrintsQuery.OrderByDescending(user => user.PrintLogs.Count(printLog => printLog.Owed)),
                "totalowedprints" => basePrintsQuery.OrderBy(user => user.PrintLogs.Count(printLog => printLog.Owed)),
                "totalowedcostDescending" => basePrintsQuery.OrderByDescending(user => user.PrintLogs.Where(printLog => printLog.Owed).Sum(printLog => printLog.Cost)),
                "totalowedcost" => basePrintsQuery.OrderBy(user => user.PrintLogs.Where(printLog => printLog.Owed).Sum(printLog => printLog.Cost)),
                _ => basePrintsQuery.OrderBy(user => user.Name.ToLower()),
            };
            
            // Return the users.
            var users = new List<UserEntry>();
            foreach (var user in basePrintsQuery.Skip(offsetUsers).Take(maxUsers).ToList())
            {
                // Get the permissions for the user.
                var permissions = new Dictionary<string, bool>();
                foreach (var permissionName in ConstructConfiguration.Configuration.Admin.ConfigurablePermissions)
                {
                    var permission = user.Permissions.FirstOrDefault(permission => permission.Name.ToLower() == permissionName.ToLower());
                    permissions[permissionName] = (permission != null && permission.IsActive());
                }
                
                // Add the user.
                users.Add(new UserEntry()
                {
                    HashedId = user.HashedId,
                    Name = user.Name,
                    Email = user.Email,
                    TotalPrints = user.PrintLogs.Count,
                    TotalWeight = user.PrintLogs.Sum(printLog => printLog.WeightGrams),
                    TotalOwedPrints = user.PrintLogs.Count(printLog => printLog.Owed),
                    TotalOwedCost = user.PrintLogs.Where(printLog => printLog.Owed).Sum(printLog => printLog.Cost),
                    Permissions = permissions,
                });
            }
            return new UsersResponse()
            {
                TotalUsers = basePrintsQuery.Count(),
                Users = users,
            };
        }
    }
}