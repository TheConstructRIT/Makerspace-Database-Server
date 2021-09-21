namespace Construct.Compatibility.Data.Response
{
    public class UserBalanceResponse : BaseLegacySuccessResponse
    {
        /// <summary>
        /// Balance of the user.
        /// </summary>
        public double? Balance { get; set; }
    }
}