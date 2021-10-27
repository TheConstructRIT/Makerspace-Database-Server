using System.Collections.Generic;
using Construct.Core.Data.Response;

namespace Construct.Admin.Data.Response
{
    public class UserEntry
    {
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
        /// Total prints that the user has printed.
        /// </summary>
        public int TotalPrints { get; set; }
        
        /// <summary>
        /// Total weight that the user has printed.
        /// </summary>
        public float TotalWeight { get; set; }
        
        /// <summary>
        /// Total owed prints for the user.
        /// </summary>
        public int TotalOwedPrints { get; set; }
        
        /// <summary>
        /// Total cost of prints for the user that are owed.
        /// </summary>
        public float TotalOwedCost { get; set; }
    }
    
    public class UsersResponse : BaseSuccessResponse
    {
        /// <summary>
        /// Total prints in the system.
        /// </summary>
        public int TotalUsers { get; set; }
        
        /// <summary>
        /// Prints in the search.
        /// </summary>
        public List<UserEntry> Users { get; set; }
    }
}