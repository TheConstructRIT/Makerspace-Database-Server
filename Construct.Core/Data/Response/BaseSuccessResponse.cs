namespace Construct.Core.Data.Response
{
    public class BaseSuccessResponse : IResponse
    {
        /// <summary>
        /// High-level status of the response.
        /// </summary>
        public string Status => "success";
    }
}