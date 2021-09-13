namespace Construct.Core.Data.Response
{
    public interface IResponse
    {
        /// <summary>
        /// High-level status of the response.
        /// </summary>
        public string Status { get; }
    }
}