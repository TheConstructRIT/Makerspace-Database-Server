using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Construct.Admin.Controllers;
using Construct.Admin.State;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Data.Response;
using Construct.Core.Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Construct.Admin.Test.Functional.Controllers
{
    public class AdminDownloadControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private AdminDownloadController _adminDownloadController;

        /// <summary>
        /// Session to use with the tests.
        /// </summary>
        private string _session = Session.GetSingleton().CreateSession("test");
        
        /// <summary>
        /// Time to use in the tests.
        /// </summary>
        private DateTime _time = DateTime.Now;

        /// <summary>
        /// Time string to use in the tests.
        /// </summary>
        private string TimeString => this._time.ToString("G", CultureInfo.CreateSpecificCulture("en-US"));
        
        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            // Create the controller.
            this._adminDownloadController = new AdminDownloadController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            
            // Add the test student user with no prints.
            this.AddData((context) =>
            {
                var user = new Core.Database.Model.User()
                {
                    HashedId = "test_hash_1",
                    Name = "Test Name 1",
                    Email = "Test Email 1",
                };
                var student = new Student()
                {
                    College = "Test College 1",
                };
            });
        }

        /// <summary>
        /// Asserts the contents of the CSVs.
        /// </summary>
        /// <param name="users">Lines of the users CSV.</param>
        /// <param name="visits">Lines of the visits CSV.</param>
        /// <param name="prints">Lines of the prints CSV.</param>
        /// <param name="totals">Lines of the totals CSV.</param>
        private void AssertCsvs(List<string> users, List<string> visits, List<string> prints, List<string> totals)
        {
            // Get and extract the CSV files.
            var csvActionResult = (PhysicalFileResult) this._adminDownloadController.GetCsvs(this._session).Result.Result;
            var tmpDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            ZipFile.ExtractToDirectory(csvActionResult.FileName, tmpDirectory);
            
            // Assert the users file is correct.
            var usersFileContent = File.ReadAllLines(Path.Combine(tmpDirectory, "LabUsers.csv"));
            Assert.AreEqual("Hashed Id,Name,Email,College,Sign-Up Year", usersFileContent[0]);
            Assert.AreEqual(users, usersFileContent.Skip(1).ToList());
            
            // Assert the users file is correct.
            var visitsFileContent = File.ReadAllLines(Path.Combine(tmpDirectory, "SwipeLog.csv"));
            Assert.AreEqual("Timestamp,Name,Email", visitsFileContent[0]);
            Assert.AreEqual(visits, visitsFileContent.Skip(1).ToList());
            
            // Assert the users file is correct.
            var printLogFileContent = File.ReadAllLines(Path.Combine(tmpDirectory, "PrintLog.csv"));
            Assert.AreEqual("Timestamp,Email,File Name,Material Type,Print Weight (g),Print Purpose,Bill To,Print Cost ($),Amount Owed ($)", printLogFileContent[0]);
            Assert.AreEqual(prints, printLogFileContent.Skip(1).ToList());
            
            // Assert the users file is correct.
            var printTotalsFileContent = File.ReadAllLines(Path.Combine(tmpDirectory, "PrintTotals.csv"));
            Assert.AreEqual("Email,Total Filament Used (g),Current Amount Owed (g),Current Amount Owed ($),Total Number Of Prints", printTotalsFileContent[0]);
            Assert.AreEqual(totals, printTotalsFileContent.Skip(1).ToList());
        }
        
        /// <summary>
        /// Tests GetCsvs with an unauthorized session.
        /// </summary>
        [Test]
        public void TestGetCsvsUnauthorized()
        {
            Assert.AreEqual("unauthorized", ((IResponse) this._adminDownloadController.GetCsvs("unknown").Result.Value).Status);
            Assert.AreEqual(this._adminDownloadController.Response.StatusCode, 401);
        }

        /// <summary>
        /// Tests GetCsvs with no data.
        /// </summary>
        [Test]
        public void TestGetCsvsEmpty()
        {
            AssertCsvs(new List<string>(), new List<string>(), new List<string>(), new List<string>());
        }

        /// <summary>
        /// Tests GetCsvs with a student with no prints.
        /// </summary>
        [Test]
        public void TestGetCsvsStudentNoPrints()
        {
            // Add a student.
            this.AddData((context) =>
            {
                var user = new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test User",
                    Email = "test@email",
                };
                context.Users.Add(user);
                context.Students.Add(new Student()
                {
                    User = user,
                    College = "Test College",
                    Year = "Test Year",
                });
            });
            
            // Assert the lines are correct.
            AssertCsvs(new List<string>() { "test_hash,Test User,test@email,Test College,Test Year", }, 
                new List<string>(), 
                new List<string>(), 
                new List<string>() { "test@email,0,0,$0.00,0", });
        }

        /// <summary>
        /// Tests GetCsvs with a user with no prints.
        /// </summary>
        [Test]
        public void TestGetCsvsUserNoPrints()
        {
            // Add a student.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test User",
                    Email = "test@email",
                });
            });
            
            // Assert the lines are correct.
            AssertCsvs(new List<string>() { "test_hash,Test User,test@email,,", }, 
                new List<string>(), 
                new List<string>(), 
                new List<string>() { "test@email,0,0,$0.00,0", });
        }

        /// <summary>
        /// Tests GetCsvs with a user with no prints with escaping.
        /// </summary>
        [Test]
        public void TestGetCsvsUserNoPrintsEscape()
        {
            // Add a student.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test, \"User",
                    Email = "test@email",
                });
            });
            
            // Assert the lines are correct.
            AssertCsvs(new List<string>() { "test_hash,\"Test, \\\"User\",test@email,,", }, 
                new List<string>(), 
                new List<string>(), 
                new List<string>() { "test@email,0,0,$0.00,0", });
        }
        
        /// <summary>
        /// Tests GetCsvs with a student with prints.
        /// </summary>
        [Test]
        public void TestGetCsvsStudentPrints()
        {
            this.AddData((context) =>
            {
                // Add the user.
                var user = new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test User",
                    Email = "test@email",
                };
                context.Users.Add(user);
                context.Students.Add(new Student()
                {
                    User = user,
                    College = "Test College",
                    Year = "Test Year",
                });
                
                // Add the prints.
                var material = new PrintMaterial()
                {
                    Name = "TestMaterial",
                };
                context.PrintMaterials.Add(material);
                context.PrintLog.Add(new PrintLog()
                {
                    User = user,
                    Time = this._time,
                    FileName = "TestFile1",
                    Material = material,
                    WeightGrams = 5,
                    Purpose = "Test Purpose",
                    Cost = 0.15f,
                    Owed = true,
                });
                context.PrintLog.Add(new PrintLog()
                {
                    User = user,
                    Time = this._time,
                    FileName = "TestFile2",
                    Material = material,
                    WeightGrams = 6,
                    Purpose = "Test Purpose",
                    Cost = 0.18f,
                    Owed = false,
                });
                context.PrintLog.Add(new PrintLog()
                {
                    User = user,
                    Time = this._time,
                    FileName = "TestFile3",
                    Material = material,
                    WeightGrams = 7,
                    Purpose = "Test Purpose",
                    Cost = 0.21f,
                    Owed = true,
                    BillTo = "TestBillTo",
                });
            });
            
            // Assert the lines are correct.
            AssertCsvs(new List<string>() { "test_hash,Test User,test@email,Test College,Test Year", }, 
                new List<string>(), 
                new List<string>()
                {
                    $"{this.TimeString},test@email,TestFile1,TestMaterial,5,Test Purpose,,$0.15,$0.15",
                    $"{this.TimeString},test@email,TestFile2,TestMaterial,6,Test Purpose,,$0.18,$0.00",
                    $"{this.TimeString},test@email,TestFile3,TestMaterial,7,Test Purpose,TestBillTo,$0.21,$0.21",
                }, 
                new List<string>() { "test@email,18,12,$0.36,3", });
        }
        
        /// <summary>
        /// Tests GetCsvs with a print with no user.
        /// </summary>
        [Test]
        public void TestGetCsvsPrintNoUser()
        {
            // Add the print.
            this.AddData((context) =>
            {
                var material = new PrintMaterial()
                {
                    Name = "TestMaterial",
                };
                context.PrintMaterials.Add(material);
                context.PrintLog.Add(new PrintLog()
                {
                    User = null,
                    Time = this._time,
                    FileName = "TestFile1",
                    Material = material,
                    WeightGrams = 5,
                    Purpose = "Test Purpose",
                    Cost = 0.15f,
                    Owed = true,
                });
            });
            
            // Assert the lines are correct.
            AssertCsvs(new List<string>(), 
                new List<string>(), 
                new List<string>()
                {
                    $"{this.TimeString},,TestFile1,TestMaterial,5,Test Purpose,,$0.15,$0.15",
                }, 
                new List<string>());
        }
        
        /// <summary>
        /// Tests GetCsvs with visit logs.
        /// </summary>
        [Test]
        public void TestGetCsvsStudentVisitLogs()
        {
            this.AddData((context) =>
            {
                // Add the user.
                var user = new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test User",
                    Email = "test@email",
                };
                context.Users.Add(user);
                context.Students.Add(new Student()
                {
                    User = user,
                    College = "Test College",
                    Year = "Test Year",
                });
                
                // Add the visit logs.
                context.VisitLogs.Add(new VisitLog()
                {
                    User = user,
                    Time = this._time,
                    Source = "Test Source",
                });
                context.VisitLogs.Add(new VisitLog()
                {
                    User = user,
                    Time = this._time,
                    Source = "Test Source",
                });
            });
            
            // Assert the lines are correct.
            AssertCsvs(new List<string>() { "test_hash,Test User,test@email,Test College,Test Year", }, 
                new List<string>() { $"{this.TimeString},Test User,test@email", $"{this.TimeString},Test User,test@email", }, 
                new List<string>(), 
                new List<string>() { "test@email,0,0,$0.00,0", });
        }
    }
}