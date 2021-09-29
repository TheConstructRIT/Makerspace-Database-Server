using Construct.Core.Data.Attribute;
using Construct.Core.Data.Request;

namespace Construct.Swipe.Data.Request
{
    public class AddRequest : BaseRequest
    {
        /// <summary>
        /// Hashed id of the user.
        /// </summary>
        [NotEmpty("missing-hashed-id")]
        public string HashedId { get; set; }
        
        /// <summary>
        /// Source of where the user swiped.
        /// </summary>
        [NotEmpty("missing-source")]
        public string Source { get; set; }
    }
}