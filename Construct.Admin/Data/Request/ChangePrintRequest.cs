using Construct.Core.Data.Attribute;
using Construct.Core.Data.Request;

namespace Construct.Admin.Data.Request
{
    public class ChangePrintRequest : BaseRequest
    {
        /// <summary>
        /// Session of the print.
        /// </summary>
        public string Session { get; set; }
        
        /// <summary>
        /// Id of the print.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// New file name of the print.
        /// </summary>
        [NotEmpty("missing-filename")]
        public string FileName { get; set; }
        
        /// <summary>
        /// New purpose of the print.
        /// </summary>
        [NotEmpty("missing-purpose")]
        public string Purpose { get; set; }
        
        /// <summary>
        /// New weight of the print.
        /// </summary>
        public float Weight { get; set; }
        
        /// <summary>
        /// New Bill To of the print.
        /// </summary>
        public string BillTo { get; set; }
        
        /// <summary>
        /// New owed flag of the print.
        /// </summary>
        public bool Owed { get; set; }
    }
}