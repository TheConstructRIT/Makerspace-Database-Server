using System;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Configuration;
using Construct.Core.Database.Model;
using Construct.Core.Receipt.Print;
using NUnit.Framework;

namespace Construct.Core.Test.Functional.Receipt.Print
{
    public class GoogleAppScriptPrintReceiptTest : BaseSqliteTest
    {
        /// <summary>
        /// Test log for the test.
        /// </summary>
        private PrintLog _testLog = new PrintLog()
        {
            User = new User()
            {
                Email = "test@email",
            },
            Time = DateTime.Now,
            FileName = "test.gcode",
            WeightGrams = 10,
            Cost = 0.3f,
            Owed = true,
        };
        
        /// <summary>
        /// Tests sending a receipt with a successful response.
        /// </summary>
        [Test]
        public void TestSuccess()
        {
            ConstructConfiguration.Configuration.PrintReceipt.GoogleAppScriptId = "valid_id";
            Assert.IsTrue(new GoogleAppScriptPrintReceipt().Send(this._testLog));
        }
        
        /// <summary>
        /// Tests sending a receipt with a failed response.
        /// </summary>
        [Test]
        public void TestError()
        {
            ConstructConfiguration.Configuration.PrintReceipt.GoogleAppScriptId = "invalid_id";
            Assert.IsFalse(new GoogleAppScriptPrintReceipt().Send(this._testLog));
        }
    }
}