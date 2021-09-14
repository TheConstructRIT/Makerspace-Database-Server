namespace Construct.User.Data.Request
{
    public class RegisterUserRequest
    {
        /// <summary>
        /// Hashed id of the user.
        /// </summary>
        public string HashedId { get; set; }
        
        /// <summary>
        /// Name of the user.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Email of the user.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// College of the user.
        /// </summary>
        public string College { get; set; }
        
        /// <summary>
        /// Year of the user at the time of sign-up.
        /// </summary>
        public string Year { get; set; }
    }
}