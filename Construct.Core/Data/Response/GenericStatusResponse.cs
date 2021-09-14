namespace Construct.Core.Data.Response
{
    public class GenericStatusResponse : IResponse
    {
        /// <summary>
        /// High-level status of the response.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Creates a generic status response.
        /// </summary>
        /// <param name="status">Status of the response.</param>
        public GenericStatusResponse(string status)
        {
            this.Status = status;
        }
    }
}