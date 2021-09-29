using Construct.Core.Data.Attribute;
using Construct.Core.Data.Request;

namespace Construct.User.Data.Request
{
    public class RegisterUserRequest : BaseRequest
    {
        /// <summary>
        /// Hashed id of the user.
        /// </summary>
        [NotEmpty("missing-hashed-id")]
        public string HashedId { get; set; }
        
        /// <summary>
        /// Name of the user.
        /// </summary>
        [NotEmpty("missing-name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Email of the user.
        /// </summary>
        [NotEmpty("missing-email")]
        public string Email { get; set; }
        
        /// <summary>
        /// College of the user.
        /// </summary>
        [NotEmpty("missing-college")]
        public string College { get; set; }
        
        /// <summary>
        /// Year of the user at the time of sign-up.
        /// </summary>
        [NotEmpty("missing-year")]
        public string Year { get; set; }
    }
}