using System.Collections.Generic;
using Construct.Core.Data.Attribute;
using Construct.Core.Data.Request;

namespace Construct.Admin.Data.Request
{
    public class ChangeUserRequest : BaseRequest
    {
        /// <summary>
        /// Session of the print.
        /// </summary>
        public string Session { get; set; }
        
        /// <summary>
        /// Hashed id of the user to change.
        /// </summary>
        [NotEmpty("missing-hashed-id")]
        public string HashedId { get; set; }
        
        /// <summary>
        /// Name of the user to set.
        /// </summary>
        [NotEmpty("missing-name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Email of the user to set.
        /// </summary>
        [NotEmpty("missing-email")]
        public string Email { get; set; }
        
        /// <summary>
        /// Permissions of the user to update.
        /// </summary>
        [NotEmpty("missing-permissions")]
        public Dictionary<string, bool> Permissions { get; set; }
    }
}