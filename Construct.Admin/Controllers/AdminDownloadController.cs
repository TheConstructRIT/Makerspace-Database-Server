using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Construct.Admin.State;
using Construct.Core.Attribute;
using Construct.Core.Data.Response;
using Construct.Core.Database.Context;
using Construct.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Construct.Admin.Controllers
{
    public class AdminDownloadController : Controller
    {
        /// <summary>
        /// Internal helper class for CSVs.
        /// </summary>
        private class CsvFile
        {
            /// <summary>
            /// Writer for the CSV.
            /// </summary>
            private readonly StreamWriter _writer;
            
            /// <summary>
            /// Creates a CSV file writer.
            /// </summary>
            /// <param name="path">Path of the CSV.</param>
            public CsvFile(string path)
            {
                this._writer = new StreamWriter(path);
            }

            /// <summary>
            /// Writes a line to the CSV.
            /// </summary>
            /// <param name="entries">Entries of the row to write.</param>
            public async Task WriteLineAsync(List<string> entries)
            {
                // Escape the lines.
                var escapedEntries = new List<string>(entries.Count);
                foreach (var entry in entries)
                {
                    if (entry == null)
                    {
                        escapedEntries.Add("");
                    }
                    else if (entry.Contains(",") || entry.Contains("\"") || entry.Contains("\n"))
                    {
                        escapedEntries.Add($"\"{entry.Replace("\"", "\\\"")}\"");
                    }
                    else
                    {
                        escapedEntries.Add(entry);
                    }
                }
                
                // Write the line.
                await this._writer.WriteLineAsync(string.Join(",", escapedEntries));
            }

            /// <summary>
            /// Closes the CSV file.
            /// </summary>
            public void Close()
            {
                this._writer.Close();
            }
        }

        /// <summary>
        /// Downloads the data of the system as CSVs.
        /// The format is based on a legacy format.
        /// </summary>
        /// <param name="session">Session of the user.</param>
        /// <returns>The CSV ZIP reference.</returns>
        [HttpGet]
        [Path("/admin/csvs")]
        public async Task<ActionResult<object>> GetPrints(string session)
        {
            // Return if the session isn't valid.
            if (!Session.GetSingleton().RefreshSession(session))
            {
                Response.StatusCode = 401;
                return new UnauthorizedResponse();
            }

            // Determine the temporary file locations.
            var temporaryDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temporaryDirectory);
            Log.Warn(temporaryDirectory);

            // Write the users CSV.
            await using var context = new ConstructContext();
            var usersCsvFile = new CsvFile(Path.Combine(temporaryDirectory, "LabUsers.csv"));
            await usersCsvFile.WriteLineAsync(new List<string>() { "Hashed Id", "Name", "Email", "College", "Sign-Up Year" });
            foreach (var user in await context.Users.ToListAsync())
            {
                var studentInformation = await context.Students.Include(student => student.User)
                    .FirstOrDefaultAsync(student => student.User.HashedId == user.HashedId);
                if (studentInformation != null)
                {
                    await usersCsvFile.WriteLineAsync(new List<string>() {
                        user.HashedId,
                        user.Name,
                        user.Email,
                        studentInformation.College,
                        studentInformation.Year });
                }
                else
                {
                    await usersCsvFile.WriteLineAsync(new List<string>()
                    {
                        user.HashedId,
                        user.Name,
                        user.Email,
                        "",
                        "",
                    });
                }
            }
            usersCsvFile.Close();
            
            // Write the swipe log CSV.
            var swipeLogCsvFile = new CsvFile(Path.Combine(temporaryDirectory, "SwipeLog.csv"));
            await swipeLogCsvFile.WriteLineAsync(new List<string>() { "Timestamp","Name","Email" });
            foreach (var swipeLog in await context.VisitLogs.Include(visiLog => visiLog.User).ToListAsync())
            {
                await swipeLogCsvFile.WriteLineAsync(new List<string>()
                {
                    swipeLog.Time.ToString("G", CultureInfo.CreateSpecificCulture("en-US")),
                    swipeLog.User.Name,
                    swipeLog.User.Email,
                });
            }
            swipeLogCsvFile.Close();
            
            // Write the print log CSV.
            var printLogCsvFile = new CsvFile(Path.Combine(temporaryDirectory, "PrintLog.csv"));
            await printLogCsvFile.WriteLineAsync(new List<string>() { "Timestamp", "Email", "File Name", "Material Type", "Print Weight (g)", "Print Purpose", "Bill To", "Print Cost ($)", "Amount Owed ($)" });
            foreach (var printLog in await context.PrintLog.Include(printLog => printLog.User).Include(printLog => printLog.Material).ToListAsync())
            {
                var costString = printLog.Cost.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                await printLogCsvFile.WriteLineAsync(new List<string>()
                {
                    printLog.Time.ToString("G", CultureInfo.CreateSpecificCulture("en-US")),
                    printLog.User == null ? "" : printLog.User.Email,
                    printLog.FileName,
                    printLog.Material.Name,
                    printLog.WeightGrams.ToString(CultureInfo.InvariantCulture),
                    printLog.Purpose,
                    printLog.BillTo,
                    costString,
                    printLog.Owed ? costString : "$0.00",
                });
            }
            printLogCsvFile.Close();
            
            // Write the print totals CSV.
            var printTotalsCsvFile = new CsvFile(Path.Combine(temporaryDirectory, "PrintTotals.csv"));
            await printTotalsCsvFile.WriteLineAsync(new List<string>() { "Email", "Total Filament Used (g)", "Current Amount Owed (g)", "Current Amount Owed ($)", "Total Number Of Prints" });
            foreach (var user in await context.Users.Include(user => user.PrintLogs).ToListAsync())
            {
                await printTotalsCsvFile.WriteLineAsync(new List<string>()
                {
                    user.Email,
                    user.PrintLogs.Sum(printLog => printLog.WeightGrams).ToString(CultureInfo.InvariantCulture),
                    user.PrintLogs.Where(printLog => printLog.Owed).Sum(printLog => printLog.WeightGrams).ToString(CultureInfo.InvariantCulture),
                    user.PrintLogs.Where(printLog => printLog.Owed).Sum(printLog => printLog.Cost).ToString("C", CultureInfo.CreateSpecificCulture("en-US")),
                    user.PrintLogs.Count.ToString(CultureInfo.InvariantCulture),
                });
            }
            printTotalsCsvFile.Close();
            
            // Compress and return the file.
            var zipFileLocation = temporaryDirectory + ".zip";
            ZipFile.CreateFromDirectory(temporaryDirectory, zipFileLocation);
            return PhysicalFile(zipFileLocation, "application/zip", "csvs.zip");
        }
    }
}