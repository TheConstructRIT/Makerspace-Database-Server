namespace Construct.Compatibility.Data.Response
{
    public class IsAuthorizedResponse : BaseLegacySuccessResponse
    {
        /// <summary>
        /// Whether the user is authorized.
        /// </summary>
        public bool Authorized { get; set; }
    }
}