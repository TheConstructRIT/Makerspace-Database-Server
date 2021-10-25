using Construct.Core.Data.Response;

namespace Construct.Admin.Data.Response
{
    public class SessionResponse : BaseSuccessResponse
    {
        /// <summary>
        /// Session for the user.
        /// </summary>
        public string Session { get; set; }
    }
}