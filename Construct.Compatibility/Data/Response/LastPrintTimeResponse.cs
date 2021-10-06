namespace Construct.Compatibility.Data.Response
{
    public class LastPrintTimeResponse : BaseLegacySuccessResponse
    {
        /// <summary>
        /// Time of the last print.
        /// </summary>
        public long? LastPrintTime { get; set; }
        
        /// <summary>
        /// Weight of the last print.
        /// </summary>
        public float? LastPrintWeight { get; set; }
    }
}