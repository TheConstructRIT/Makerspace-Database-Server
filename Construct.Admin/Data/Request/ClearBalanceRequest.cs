using Construct.Core.Data.Attribute;
using Construct.Core.Data.Request;

namespace Construct.Admin.Data.Request
{
    public class ClearBalanceRequest : BaseRequest
    {
        /// <summary>
        /// Session of the print.
        /// </summary>
        public string Session { get; set; }
        
        /// <summary>
        /// Hashed id of the user to clear.
        /// </summary>
        [NotEmpty("missing-hashed-id")]
        public string HashedId { get; set; }
    }
}