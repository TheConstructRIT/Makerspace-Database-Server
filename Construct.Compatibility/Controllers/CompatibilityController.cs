using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Construct.Compatibility.Data.Response;
using Construct.Core.Attribute;
using Construct.Core.Configuration;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.User.Data.Correction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.Compatibility.Controllers
{
    public class CompatibilityController : Controller
    {
        /// <summary>
        /// Returns the hash to use.
        /// </summary>
        /// <param name="hashedId">Hashed id to use, if any.</param>
        /// <param name="universityId">University id to use, if any.</param>
        private static string GetHash(string hashedId, string universityId)
        {
            if (hashedId == null && universityId != null)
            {
                // Has the id.
                using var sha256 = new SHA256Managed();
                return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(universityId))).Replace("-", "").ToLower();
            }
            else
            {
                // Return the original hashed id.
                return hashedId;
            }
        }
        
        /// <summary>
        /// Legacy endpoint for getting the name of a user.
        /// </summary>
        [HttpGet]
        [Path("/name")]
        public async Task<NameResponse> GetName(string hashedId, string universityId)
        {
            // Convert the university id to a hash.
            hashedId = GetHash(hashedId, universityId);
            
            // Get and return the name.
            await using var context = new ConstructContext();
            var user = await context.Users.FirstOrDefaultAsync((u => u.HashedId == hashedId));
            return new NameResponse()
            {
                Name = user?.Name,
            };
        }
        
        /// <summary>
        /// Legacy endpoint for getting the balance of a user.
        /// </summary>
        [HttpGet]
        [Path("/userbalance")]
        public async Task<UserBalanceResponse> GetUserBalance(string hashedId, string universityId)
        {
            // Convert the university id to a hash.
            hashedId = GetHash(hashedId, universityId);

            // Get the user and return if the user doesn't exist.
            await using var context = new ConstructContext();
            var user = await context.Users.Include(u => u.PrintLogs).FirstOrDefaultAsync(u => u.HashedId == hashedId);
            if (user == null)
            {
                return new UserBalanceResponse();
            }
            
            // Determine and return the balance for the print logs.
            var owedPrints = user.PrintLogs.Where(print => print.Owed);
            var owedPrintBalance = 0.0;
            foreach (var print in owedPrints)
            {
                owedPrintBalance += print.Cost;
            }
            return new UserBalanceResponse()
            {
                Balance = owedPrintBalance,
            };
        }
        
        // TODO: GET /isauthorized (Cura)
        
        // TODO: GET /hashedid (Cura)
        
        // TODO: GET /userinfo (Cura)

        /// <summary>
        /// Legacy endpoint for adding a user.
        /// </summary>
        [HttpPost]
        [Path("/appenduser")]
        public async Task<BaseSuccessResponse> AppendUser(string hashedId, string universityId, string name, string email, string college, string year)
        {
            // Convert the university id to a hash.
            hashedId = GetHash(hashedId, universityId);
            
            // Correct the email.
            var emailCorrection = new EmailCorrection()
            {
                ValidEmails = ConstructConfiguration.Configuration.Email.ValidEmails,
                Corrections = ConstructConfiguration.Configuration.Email.EmailCorrections,
            };
            try
            {
                email = emailCorrection.CorrectEmail(email);
            }
            catch (FormatException)
            {
                
            }
            
            // Add the user and return success.
            await using var context = new ConstructContext();
            var user = new Core.Database.Model.User()
            {
                HashedId = hashedId.ToLower(),
                Name = name,
                Email = email,
                SignUpTime = DateTime.Now,
            };
            var student = new Student()
            {
                User = user,
                College = college,
                Year = year,
            };
            context.Users.Add(user);
            context.Students.Add(student);
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }
        
        /// <summary>
        /// Legacy endpoint for adding a swipe log.
        /// </summary>
        [HttpPost]
        [Path("/appendswipelog")]
        public async Task<BaseSuccessResponse> AppendSwipeLog(string hashedId, string universityId)
        {
            // Convert the university id to a hash.
            hashedId = GetHash(hashedId, universityId);
            
            // Add the swipe log and return success.
            await using var context = new ConstructContext();
            var user = await context.Users.FirstOrDefaultAsync(user => user.HashedId == hashedId);
            if (user == null)
            {
                return new BaseSuccessResponse();
            }
            context.VisitLogs.Add(new VisitLog()
            {
                User = user,
                Time = DateTime.Now,
                Source = "MainLab",
            });
            await context.SaveChangesAsync();
            return new BaseSuccessResponse();
        }

        // TODO: GET /lastprinttime (Cura)
        
        // TODO: POST /appendprint (Cura)
    }
}