using Construct.Core.Data.Attribute;
using Construct.Core.Data.Request;

namespace Construct.Print.Data.Request
{
    public class AddPrintRequest : BaseRequest
    {
        /// <summary>
        /// Hashed id of the user.
        /// </summary>
        [NotEmpty("missing-hashed-id")]
        public string HashedId { get; set; }
        
        /// <summary>
        /// File name of the print.
        /// </summary>
        [NotEmpty("missing-file-name")]
        public string FileName { get; set; }
        
        /// <summary>
        /// Material of the print.
        /// </summary>
        [NotEmpty("missing-material")]
        public string Material { get; set; }
        
        /// <summary>
        /// Weight of the print in grams.
        /// </summary>
        public float Weight { get; set; }
        
        /// <summary>
        /// Purpose of the print.
        /// </summary>
        [NotEmpty("missing-purpose")]
        public string Purpose { get; set; }
        
        /// <summary>
        /// Who to bill for the print. May be null.
        /// </summary>
        public string BillTo { get; set; }
        
        /// <summary>
        /// Whether the print is owed. True is assumed if not provided.
        /// </summary>
        public bool? Owed { get; set; }
    }
}