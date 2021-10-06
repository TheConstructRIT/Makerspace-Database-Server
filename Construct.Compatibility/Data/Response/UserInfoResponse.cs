namespace Construct.Compatibility.Data.Response
{
    public class UserInfoResponse : BaseLegacySuccessResponse
    {
        /// <summary>
        /// Email of the user.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Last print purpose entered by the user.
        /// </summary>
        public string LastPurpose { get; set; }
        
        /// <summary>
        /// Last MSD number used by the user.
        /// </summary>
        public string LastMSDNumber { get; set; }
    }
}