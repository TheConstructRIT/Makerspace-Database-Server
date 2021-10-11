using System;
using Construct.Core.Configuration;
using Construct.Core.Database.Model;

namespace Construct.Core.Receipt.Print
{
    public static class PrintReceiptProvider
    {
        /// <summary>
        /// Creates a print receipt provider for the current configuration.
        /// </summary>
        /// <returns>The print receipt provider for the current configuration.</returns>
        public static IPrintReceipt GetProvider()
        {
            var provider = ConstructConfiguration.Configuration.PrintReceipt?.Provider?.ToLower();
            switch (provider)
            {
                case("googleappscripts"):
                    return new GoogleAppScriptPrintReceipt();
                default:
                    throw new InvalidOperationException("Invalid print receipt provider: " + provider);
            }
        }

        /// <summary>
        /// Sends a print receipt for the given entry.
        /// </summary>
        /// <param name="printLog">The entry to send the receipt for.</param>
        /// <returns>Whether the receipt was sent.</returns>
        public static bool SendReceipt(PrintLog printLog)
        {
            return GetProvider().Send(printLog);
        }
    }
}