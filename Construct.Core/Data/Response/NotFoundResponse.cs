namespace Construct.Core.Data.Response
{
    public class NotFoundResponse : IResponse
    {
        /// <summary>
        /// High-level status of the response.
        /// </summary>
        public string Status => "not-found";
    }
}