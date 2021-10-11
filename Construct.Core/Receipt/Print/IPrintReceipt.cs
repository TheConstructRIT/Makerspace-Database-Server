using Construct.Core.Database.Model;

namespace Construct.Core.Receipt.Print
{
    public interface IPrintReceipt
    {
        /// <summary>
        /// Sends a receipt for a print entry.
        /// </summary>
        /// <param name="log">Print log to send.</param>
        /// <returns>Whether the receipt was sent.</returns>
        public bool Send(PrintLog log);
    }
}