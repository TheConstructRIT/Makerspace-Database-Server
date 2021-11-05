using System.Collections.Generic;
using Construct.Core.Data.Response;

namespace Construct.User.Data.Response
{
    public class GetUserResponse : BaseSuccessResponse
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
        /// Owed print balance of the user.
        /// </summary>
        public double OwedPrintBalance { get; set; }
        
        /// <summary>
        /// Owed print balance of the user.
        /// </summary>
        public List<string> Permissions { get; set; }
    }
}