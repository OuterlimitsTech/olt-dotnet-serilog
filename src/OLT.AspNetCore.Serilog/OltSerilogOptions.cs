namespace OLT.Logging.Serilog
{
    public class OltSerilogOptions
    {
        public bool ShowExceptionDetails { get; set; }

        /// <summary>
        /// Limit of size (in MB) <seealso cref="OltMiddlewarePayload"/> will write to the log for the Request/Response Body
        /// </summary>
        public double BodyPayloadLimit { get; set; } = 0.25;

        public string ErrorMessage { get; set; } = "An error has occurred.";
    }
}
