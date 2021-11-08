using Construct.Core.Data.Response;

namespace Construct.Print.Data.Response
{
    public class LastPrintResponse : BaseSuccessResponse
    {
        /// <summary>
        /// File name of the last print.
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// Time of the last print.
        /// </summary>
        public long TimeStamp { get; set; }
        
        /// <summary>
        /// Purpose of the last print.
        /// </summary>
        public string Purpose { get; set; }
        
        /// <summary>
        /// Bill to of the last print.
        /// </summary>
        public string BillTo { get; set; }
    }
}