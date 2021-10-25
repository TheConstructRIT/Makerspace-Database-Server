namespace Construct.Core.Data.Response
{
    public class UnauthorizedResponse: IResponse
    {
        /// <summary>
        /// High-level status of the response.
        /// </summary>
        public string Status => "unauthorized";
    }
}