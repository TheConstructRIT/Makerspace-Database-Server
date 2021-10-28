using System.Collections.Generic;
using Construct.Core.Data.Response;

namespace Construct.Admin.Data.Response
{
    public class PrintResponseEntryPrint
    {
        /// <summary>
        /// Id of the print.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Name of the print.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Unix timestamp of the print.
        /// </summary>
        public long Timestamp { get; set; }
        
        /// <summary>
        /// Name of the material of the print.
        /// </summary>
        public string Material { get; set; }
        
        /// <summary>
        /// Weight of the print.
        /// </summary>
        public float Weight { get; set; }
        
        /// <summary>
        /// Purpose of the print.
        /// </summary>
        public string Purpose { get; set; }
        
        /// <summary>
        /// Bill To of the print.
        /// </summary>
        public string BillTo { get; set; }
        
        /// <summary>
        /// Cost of the print.
        /// </summary>
        public float Cost { get; set; }
        
        /// <summary>
        /// Whether the print is owed.
        /// </summary>
        public bool Owed { get; set; }
    }
    
    public class PrintResponseEntryUser
    {
        /// <summary>
        /// Name of the user.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Email of the user.
        /// </summary>
        public string Email { get; set; }
    }
    
    public class PrintResponseEntry
    {
        /// <summary>
        /// Data of the print.
        /// </summary>
        public PrintResponseEntryPrint Print { get; set; }
        
        /// <summary>
        /// Data of the user.
        /// </summary>
        public PrintResponseEntryUser User { get; set; }
    }
    
    public class PrintsResponse : BaseSuccessResponse
    {
        /// <summary>
        /// Total prints in the system.
        /// </summary>
        public int TotalPrints { get; set; }
        
        /// <summary>
        /// Prints in the search.
        /// </summary>
        public List<PrintResponseEntry> Prints { get; set; }
    }
}