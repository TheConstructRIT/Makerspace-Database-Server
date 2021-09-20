namespace Construct.Swipe.Data.Request
{
    public class AddRequest
    {
        /// <summary>
        /// Hashed id of the user.
        /// </summary>
        public string HashedId { get; set; }
        
        /// <summary>
        /// Source of where the user swiped.
        /// </summary>
        public string Source { get; set; }
    }
}