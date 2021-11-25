using System.Collections.Generic;
using Construct.Core.Data.Response;

namespace Construct.Admin.Data.Response
{
    public class VisitEntry
    {
        /// <summary>
        /// Time of the visit.
        /// </summary>
        public long Timestamp { get; set; }
        
        /// <summary>
        /// Source of the visit.
        /// </summary>
        public string Source { get; set; }
        
        /// <summary>
        /// Hashed id of the user.
        /// </summary>
        public string HashedId { get; set; }
        
        /// <summary>
        /// Name of the user.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Email of the user.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Total owed prints for the user.
        /// </summary>
        public int TotalOwedPrints { get; set; }
        
        /// <summary>
        /// Total cost of prints for the user that are owed.
        /// </summary>
        public float TotalOwedCost { get; set; }
        
        /// <summary>
        /// Permissions of the user.
        /// </summary>
        public Dictionary<string , bool> Permissions { get; set; }
    }
    
    public class VisitsResponse : BaseSuccessResponse
    {
        /// <summary>
        /// Total visits in the system.
        /// </summary>
        public int TotalVisits { get; set; }
        
        /// <summary>
        /// Visits in the search.
        /// </summary>
        public List<VisitEntry> Visits { get; set; }
    }
}