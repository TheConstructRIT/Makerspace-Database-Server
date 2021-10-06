namespace Construct.Compatibility.Data.Response
{
    public class HashedIdResponse : BaseLegacySuccessResponse
    {
        /// <summary>
        /// Hashed if of the user.
        /// </summary>
        public string HashedId { get; set; }
    }
}