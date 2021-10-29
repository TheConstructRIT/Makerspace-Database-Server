using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Admin.Controllers;
using Construct.Admin.Data.Response;
using Construct.Admin.State;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Construct.Admin.Test.Functional.Controllers
{
    public class AdminSearchControllerTest : BaseSqliteTest
    {
        /// <summary>
        /// Controller under test.
        /// </summary>
        private AdminSearchController _adminSearchController;

        /// <summary>
        /// Session to use with the tests.
        /// </summary>
        private string _session = Session.GetSingleton().CreateSession("test");
        
        /// <summary>
        /// Sets up the controller.
        /// </summary>
        [SetUp]
        public void SetUpController()
        {
            // Create the controller.
            this._adminSearchController = new AdminSearchController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            
            // Add the test users and prints.
            this.AddData((context) =>
            {
                // Add 9 normal prints.
                for (var i = 0; i < 9; i++)
                {
                    // Create the test user, material, and print.
                    // All of the strings are offset for various tests with searches.
                    var testUser = new Core.Database.Model.User()
                    {
                        HashedId = "test_hash_" + i,
                        Name = "Test Name " + i,
                        Email = "test" + i + "@email",
                        SignUpTime = DateTime.Now,
                    };
                    var testMaterial = new PrintMaterial()
                    {
                        Name = "TestMaterial" + ((i + 1) % 9),
                        CostPerGram = 0.03f,
                    };
                    var testPrint = new PrintLog()
                    {
                        User = testUser,
                        Time = new DateTime(((i + 2) % 9) * 1000),
                        FileName = "TestPrint" + ((i + 3) % 9),
                        Material = testMaterial,
                        WeightGrams = ((i + 4) % 10),
                        Purpose = "TestPurpose" + ((i + 5) % 9),
                        BillTo = "TestBillTo" + ((i + 6) % 9),
                        Cost = ((i + 7) % 9),
                        Owed = (i % 2 == 0),
                    };
                    
                    // Add the test data.
                    context.Users.Add(testUser);
                    context.PrintMaterials.Add(testMaterial);
                    context.PrintLog.Add(testPrint);
                }
                
                // Add a print with no user.
                var noUserTestMaterial = new PrintMaterial()
                {
                    Name = "TestMaterial9",
                    CostPerGram = 0.03f,
                };
                var noUserTestPrint = new PrintLog()
                {
                    User = null,
                    Time = new DateTime(9000),
                    FileName = "TestPrint9",
                    Material = noUserTestMaterial,
                    WeightGrams = 9,
                    Purpose = "TestPurpose9",
                    BillTo = "TestBillTo9",
                    Cost = 9,
                    Owed = true,
                };
                context.PrintMaterials.Add(noUserTestMaterial);
                context.PrintLog.Add(noUserTestPrint);
            });
        }

        /// <summary>
        /// Asserts that a list of prints are returned for a search.
        /// </summary>
        /// <param name="order">Column to sort by.</param>
        /// <param name="ascending">Whether to search by ascending.</param>
        /// <param name="printNames">Print names to assert.</param>
        /// <param name="offsetPrints">Offset of the prints to pull.</param>
        /// <param name="search">String to search for.</param>
        /// <param name="hashedId">Hashed id to filter for.</param>
        private void AssertPrintsOrder(string order, bool ascending, List<string> printNames, int offsetPrints = 0, string search = "", string hashedId = null)
        {
            var response = (PrintsResponse) this._adminSearchController.GetPrints(this._session, 3, offsetPrints, order, ascending, search, hashedId).Result.Value;
            Assert.AreEqual(printNames, response.Prints.Select(print => print.Print.Name).ToList());
            Assert.AreEqual(10, response.TotalPrints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order">Column to sort by.</param>
        /// <param name="ascending">Whether to search by ascending.</param>
        /// <param name="userNames">User names to assert.</param>
        /// <param name="offsetUsers">Offset of the users to return.</param>
        /// <param name="search">String to search for.</param>
        private void AssertUsersOrder(string order, bool ascending, List<string> userNames, int offsetUsers = 0, string search = "")
        {
            var response = (UsersResponse) this._adminSearchController.GetUsers(this._session, 3, offsetUsers, order, ascending, search).Result.Value;
            Assert.AreEqual(userNames, response.Users.Select(print => print.Name).ToList());
            Assert.AreEqual(9, response.TotalUsers);
        }

        /// <summary>
        /// Tests GetPrints with an unauthorized search.
        /// </summary>
        [Test]
        public void TestGetPrintsUnauthorized()
        {
            Assert.AreEqual("unauthorized", this._adminSearchController.GetPrints("unknown", 10, 0, null).Result.Value.Status);
        }

        /// <summary>
        /// Tests GetPrints with ordering by time ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsTimeAscending()
        {
            this.AssertPrintsOrder("Time", true, new List<string>() { "TestPrint1", "TestPrint2", "TestPrint3" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by time descending.
        /// </summary>
        [Test]
        public void TestGetPrintsTimeDescending()
        {
            this.AssertPrintsOrder("Time", false, new List<string>() { "TestPrint9", "TestPrint0", "TestPrint8" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by file name ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsFileNameAscending()
        {
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint0", "TestPrint1", "TestPrint2" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by file name descending.
        /// </summary>
        [Test]
        public void TestGetPrintsFileNameDescending()
        {
            this.AssertPrintsOrder("FileName", false, new List<string>() { "TestPrint9", "TestPrint8", "TestPrint7" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by purpose ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsPurposeAscending()
        {
            this.AssertPrintsOrder("Purpose", true, new List<string>() { "TestPrint7", "TestPrint8", "TestPrint0" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by purpose descending.
        /// </summary>
        [Test]
        public void TestGetPrintsPurposeDescending()
        {
            this.AssertPrintsOrder("Purpose", false, new List<string>() { "TestPrint9", "TestPrint6", "TestPrint5" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by material ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsMaterialAscending()
        {
            this.AssertPrintsOrder("Material", true, new List<string>() { "TestPrint2", "TestPrint3", "TestPrint4" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by material descending.
        /// </summary>
        [Test]
        public void TestGetPrintsMaterialDescending()
        {
            this.AssertPrintsOrder("Material", false, new List<string>() { "TestPrint9", "TestPrint1", "TestPrint0" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by weight ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsWeightAscending()
        {
            this.AssertPrintsOrder("Weight", true, new List<string>() { "TestPrint0", "TestPrint1", "TestPrint2" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by weight descending.
        /// </summary>
        [Test]
        public void TestGetPrintsWeightDescending()
        {
            this.AssertPrintsOrder("Weight", false, new List<string>() { "TestPrint8", "TestPrint9", "TestPrint7" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by cost ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsCostAscending()
        {
            this.AssertPrintsOrder("Cost", true, new List<string>() { "TestPrint5", "TestPrint6", "TestPrint7" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by cost descending.
        /// </summary>
        [Test]
        public void TestGetPrintsCostDescending()
        {
            this.AssertPrintsOrder("Cost", false, new List<string>() { "TestPrint9", "TestPrint4", "TestPrint3" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by owed ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsOwedAscending()
        {
            this.AssertPrintsOrder("Owed", true, new List<string>() { "TestPrint4", "TestPrint6", "TestPrint8" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by owed descending.
        /// </summary>
        [Test]
        public void TestGetPrintsOwedDescending()
        {
            this.AssertPrintsOrder("Owed", false, new List<string>() { "TestPrint3", "TestPrint5", "TestPrint7" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by Bill To ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsBillToAscending()
        {
            this.AssertPrintsOrder("BillTo", true, new List<string>() { "TestPrint6", "TestPrint7", "TestPrint8" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by Bill To descending.
        /// </summary>
        [Test]
        public void TestGetPrintsBillToDescending()
        {
            this.AssertPrintsOrder("BillTo", false, new List<string>() { "TestPrint9", "TestPrint5", "TestPrint4" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by user ascending.
        /// </summary>
        [Test]
        public void TestGetPrintsUserAscending()
        {
            this.AssertPrintsOrder("User", true, new List<string>() { "TestPrint9", "TestPrint3", "TestPrint4" });
        }

        /// <summary>
        /// Tests GetPrints with ordering by user descending.
        /// </summary>
        [Test]
        public void TestGetPrintsUserDescending()
        {
            this.AssertPrintsOrder("User", false, new List<string>() { "TestPrint2", "TestPrint1", "TestPrint0" });
        }
        
        /// <summary>
        /// Tests GetPrints with an unknown order.
        /// </summary>
        [Test]
        public void TestGetPrintsUnknownOrder()
        {
            this.AssertPrintsOrder("Unknown", true, new List<string>() { "TestPrint0", "TestPrint1", "TestPrint2" });
        }

        /// <summary>
        /// Tests GetPrints with an offset.
        /// </summary>
        [Test]
        public void TestGetPrintsOffset()
        {
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint0", "TestPrint1", "TestPrint2" }, 0);
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint1", "TestPrint2", "TestPrint3" }, 1);
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint4", "TestPrint5", "TestPrint6" }, 4);
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint8", "TestPrint9" }, 8);
            this.AssertPrintsOrder("FileName", true, new List<string>() { }, 10);
        }

        /// <summary>
        /// Tests GetPrints with a search term.
        /// </summary>
        [Test]
        public void TestGetPrintsSearch()
        {
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint1" }, search: "TestPrint1");
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint7" }, search: "TestBillTo1");
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint1", "TestPrint7" }, search: "1");
            this.AssertPrintsOrder("FileName", true, new List<string>() { }, search: "unknown");
        }

        /// <summary>
        /// Tests GetPrints with a hashed id.
        /// </summary>
        [Test]
        public void TestGetPrintsHashedId()
        {
            this.AssertPrintsOrder("FileName", true, new List<string>() { "TestPrint4" }, hashedId: "test_hash_1");
            this.AssertPrintsOrder("FileName", true, new List<string>() { }, hashedId: "unknown");
        }

        /// <summary>
        /// Tests GetUsers with an unauthorized search.
        /// </summary>
        [Test]
        public void TestGetUsersUnauthorized()
        {
            Assert.AreEqual("unauthorized", this._adminSearchController.GetUsers("unknown", 10, 0, null).Result.Value.Status);
        }

        /// <summary>
        /// Tests GetUsers with ordering by name ascending.
        /// </summary>
        [Test]
        public void TestGetUsersNameAscending()
        {
            this.AssertUsersOrder("Name", true, new List<string>() { "Test Name 0", "Test Name 1", "Test Name 2" });
        }

        /// <summary>
        /// Tests GetUsers with ordering by name descending.
        /// </summary>
        [Test]
        public void TestGetUsersNameDescending()
        {
            this.AssertUsersOrder("Name", false, new List<string>() { "Test Name 8", "Test Name 7", "Test Name 6" });
        }
        
        /// <summary>
        /// Tests GetUsers with ordering by email ascending.
        /// </summary>
        [Test]
        public void TestGetUsersEmailAscending()
        {
            this.AssertUsersOrder("Email", true, new List<string>() { "Test Name 0", "Test Name 1", "Test Name 2" });
        }

        /// <summary>
        /// Tests GetUsers with ordering by email descending.
        /// </summary>
        [Test]
        public void TestGetUsersEmailDescending()
        {
            this.AssertUsersOrder("Email", false, new List<string>() { "Test Name 8", "Test Name 7", "Test Name 6" });
        }
        
        /// <summary>
        /// Tests GetUsers with ordering by total prints ascending.
        /// </summary>
        [Test]
        public void TestGetUsersTotalPrintsAscending()
        {
            this.AssertUsersOrder("TotalPrints", true, new List<string>() { "Test Name 2", "Test Name 4", "Test Name 5" });
        }

        /// <summary>
        /// Tests GetUsers with ordering by total prints descending.
        /// </summary>
        [Test]
        public void TestGetUsersTotalPrintsDescending()
        {
            this.AssertUsersOrder("TotalPrints", false, new List<string>() { "Test Name 2", "Test Name 4", "Test Name 5" });
        }

        /// <summary>
        /// Tests GetUsers with ordering by total weight ascending.
        /// </summary>
        [Test]
        public void TestGetUsersTotalWeightAscending()
        {
            this.AssertUsersOrder("TotalWeight", true, new List<string>() { "Test Name 6", "Test Name 7", "Test Name 8" });
        }

        /// <summary>
        /// Tests GetUsers with ordering by total weight descending.
        /// </summary>
        [Test]
        public void TestGetUsersTotalWeightDescending()
        {
            this.AssertUsersOrder("TotalWeight", false, new List<string>() { "Test Name 5", "Test Name 4", "Test Name 3" });
        }

        /// <summary>
        /// Tests GetUsers with ordering by total owed prints ascending.
        /// </summary>
        [Test]
        public void TestGetUsersTotalOwedPrintsAscending()
        {
            this.AssertUsersOrder("TotalOwedPrints", true, new List<string>() { "Test Name 1", "Test Name 5", "Test Name 7" });
        }

        /// <summary>
        /// Tests GetUsers with ordering by total owed prints descending.
        /// </summary>
        [Test]
        public void TestGetUsersTotalOwedPrintsDescending()
        {
            this.AssertUsersOrder("TotalOwedPrints", false, new List<string>() { "Test Name 2", "Test Name 4", "Test Name 6" });
        }
        
        /// <summary>
        /// Tests GetUsers with ordering by total owed cost ascending.
        /// </summary>
        [Test]
        public void TestGetUsersTotalOwedCostAscending()
        {
            this.AssertUsersOrder("TotalOwedCost", true, new List<string>() { "Test Name 1", "Test Name 2", "Test Name 5" });
        }

        /// <summary>
        /// Tests GetUsers with ordering by total owed cost descending.
        /// </summary>
        [Test]
        public void TestGetUsersTotalOwedCostDescending()
        {
            this.AssertUsersOrder("TotalOwedCost", false, new List<string>() { "Test Name 0", "Test Name 8", "Test Name 6" });
        }

        /// <summary>
        /// Tests GetUsers with an unknown order.
        /// </summary>
        [Test]
        public void TestGetUsersUnknownOrder()
        {
            this.AssertUsersOrder("Unknown", true, new List<string>() { "Test Name 0", "Test Name 1", "Test Name 2" });
        }
        
        /// <summary>
        /// Tests GetUsers with an offset.
        /// </summary>
        [Test]
        public void TestGetUsersOffset()
        {
            this.AssertUsersOrder("Name", true, new List<string>() { "Test Name 0", "Test Name 1", "Test Name 2" }, 0);
            this.AssertUsersOrder("Name", true, new List<string>() { "Test Name 1", "Test Name 2", "Test Name 3" }, 1);
            this.AssertUsersOrder("Name", true, new List<string>() { "Test Name 4", "Test Name 5", "Test Name 6" }, 4);
            this.AssertUsersOrder("Name", true, new List<string>() { "Test Name 7", "Test Name 8" }, 7);
            this.AssertUsersOrder("Name", true, new List<string>() { }, 10);
        }

        /// <summary>
        /// Tests GetUsers with a search term.
        /// </summary>
        [Test]
        public void TestGetUsersSearch()
        {
            this.AssertUsersOrder("Name", true, new List<string>() { "Test Name 1" }, search: "Test Name 1");
            this.AssertUsersOrder("Name", true, new List<string>() { "Test Name 1" }, search: "test1@email");
            this.AssertUsersOrder("Name", true, new List<string>() { "Test Name 1"}, search: "1");
            this.AssertUsersOrder("Name", true, new List<string>() { }, search: "unknown");
        }

        /// <summary>
        /// Tests GetUsers for permissions.
        /// </summary>
        [Test]
        public void TestGetUsersPermissions()
        {
            // Make the 1st user a LabManager and the 2nd user a LabManager, but expired.
            this.AddData((context) =>
            {
                context.Users.Include(user => user.Permissions).First(user => user.HashedId == "test_hash_1").Permissions.Add(new Permission()
                {
                    Name = "LabManager",
                });
                context.Users.Include(user => user.Permissions).First(user => user.HashedId == "test_hash_2").Permissions.Add(new Permission()
                {
                    Name = "LabManager",
                    EndTime = new DateTime(0),
                });
            });
            
            // Check that the permissions are correct.
            Assert.IsTrue(((UsersResponse) this._adminSearchController.GetUsers(this._session, 1, 0, "Name", search: "Test Name 1").Result.Value).Users[0].Permissions["LabManager"]);
            Assert.IsFalse(((UsersResponse) this._adminSearchController.GetUsers(this._session, 1, 0, "Name", search: "Test Name 2").Result.Value).Users[0].Permissions["LabManager"]);
            Assert.IsFalse(((UsersResponse) this._adminSearchController.GetUsers(this._session, 1, 0, "Name", search: "Test Name 3").Result.Value).Users[0].Permissions["LabManager"]);
        }
    }
}