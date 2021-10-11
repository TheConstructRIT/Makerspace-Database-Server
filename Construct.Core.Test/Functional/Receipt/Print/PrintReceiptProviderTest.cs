using System;
using Construct.Base.Test.Functional.Base;
using Construct.Core.Configuration;
using Construct.Core.Database.Model;
using Construct.Core.Receipt.Print;
using NUnit.Framework;

namespace Construct.Core.Test.Functional.Receipt.Print
{
    public class PrintReceiptProviderTest : BaseSqliteTest
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
        /// Tests getting print receipt providers.
        /// </summary>
        [Test]
        public void TestGetProvider()
        {
            ConstructConfiguration.Configuration.PrintReceipt.Provider = "GoogleAppScripts";
            Assert.That(PrintReceiptProvider.GetProvider() is GoogleAppScriptPrintReceipt);
            ConstructConfiguration.Configuration.PrintReceipt.Provider = "unknown";
            Assert.Throws<InvalidOperationException>(() => PrintReceiptProvider.GetProvider());
        }
        
        /// <summary>
        /// Tests sending print receipts with Google App Scripts.
        /// </summary>
        [Test]
        public void TestGoogleAppScripts()
        {
            ConstructConfiguration.Configuration.PrintReceipt.Provider = "GoogleAppScripts";
            ConstructConfiguration.Configuration.PrintReceipt.GoogleAppScriptId = "valid_id";
            Assert.IsTrue(PrintReceiptProvider.SendReceipt(this._testLog));
        }
    }
}