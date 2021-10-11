using System.Linq;
using System.Net.Http;
using Construct.Core.Configuration;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.Core.Logging;
using Microsoft.EntityFrameworkCore;

namespace Construct.Core.Receipt.Print
{
    public class GoogleAppScriptPrintReceipt : IPrintReceipt
    {
        /// <summary>
        /// HTTP Client used for sending requests.
        /// Intended to only be used by unit tests.
        /// </summary>
        public static HttpClient Client { get; set; } = new HttpClient();

        /// <summary>
        /// Sends a receipt for a print entry.
        /// </summary>
        /// <param name="log">Print log to send.</param>
        /// <returns>Whether the receipt was sent.</returns>
        public bool Send(PrintLog log)
        {
            // Get the user prints.
            using var context = new ConstructContext();
            var totalPrints = context.PrintLog.Include(printLog => printLog.User)
                .Count(printLog => printLog.User.HashedId == log.User.HashedId);
            var totalOwedBalance = context.PrintLog.Include(printLog => printLog.User)
                .Where(printLog => printLog.User.HashedId == log.User.HashedId && log.Owed == false)
                .Sum(printLog => printLog.Cost);
            
            // Send the request.
            Log.Debug($"Sending print receipt for {log.FileName}");
            var scriptId = ConstructConfiguration.Configuration.PrintReceipt.GoogleAppScriptId;
            var url = $"https://script.google.com/macros/s/{scriptId}/exec?request=sendemail&email={log.User.Email}&printCount={totalPrints}&fileName={log.FileName}&printWeight={log.WeightGrams}&printCost={log.Cost}&totalCost={totalOwedBalance}";
            var emailResult = Client.PostAsync(url, new StringContent("")).Result;
            if ((int) emailResult.StatusCode >= 400)
            {
                Log.Error($"Failed to send print receipt for {log.FileName}\n{emailResult.Content.ReadAsStringAsync().Result}");
                return false;
            }
            else
            {
                Log.Info($"Send print receipt for {log.FileName}");
                return true;
            }
        }
    }
}