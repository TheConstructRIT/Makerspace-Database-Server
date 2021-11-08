using System;
using System.Net;
using Construct.Base.Test.Integration.Base;
using Construct.Core.Data.Response;
using Construct.Print.Data.Request;
using Construct.Print.Data.Response;
using NUnit.Framework;

namespace Construct.Print.Test.Integration.Controllers
{
    public class PrintControllerTest : BaseIntegrationTest
    {
        /// <summary>
        /// Starts the program.
        /// </summary>
        [SetUp]
        public void SetUpProgram()
        {
            // Start the program.
            this.StartProgram<Program>();
            this.WaitForApp("Print");
            
            // Add a test user.
            this.AddData((context) =>
            {
                context.Users.Add(new Core.Database.Model.User()
                {
                    HashedId = "test_hash",
                    Name = "Test Name",
                    Email = "test@email",
                    SignUpTime = DateTime.Now,
                });
            });
        }

        /// <summary>
        /// Tests adding a print.
        /// </summary>
        [Test]
        public void TestAddPrint()
        {
            // Add a print for the user.
            var addRequest = new AddPrintRequest()
            {
                HashedId = "test_hash",
                FileName = "test_file",
                Material = "Test",
                Weight = 10,
                Purpose = "Test Purpose",
            };
            var (addResponse, addResponseCode) = this.Post<GenericStatusResponse>("Print", "/print/add", addRequest);
            Assert.AreEqual(HttpStatusCode.OK, addResponseCode);
            Assert.AreEqual("success",addResponse.Status);
            
            // Assert that the last print is the same.
            var (lastPrintResponse, lastPrintResponseCode) = this.Get<LastPrintResponse>("Print", "/print/last?hashedid=test_hash");
            Assert.AreEqual(HttpStatusCode.OK, lastPrintResponseCode);
            Assert.AreEqual("success",lastPrintResponse.Status);
            Assert.AreEqual("test_file",lastPrintResponse.FileName);
            Assert.AreEqual(10,lastPrintResponse.Weight);
            Assert.AreEqual("Test Purpose",lastPrintResponse.Purpose);
            Assert.IsNull(lastPrintResponse.BillTo);
            
            // Stop the program.
            // Can't be done in a TearDown since the ASP.NET app prevents it from running.
            this.StopProgram();
        }
    }
}