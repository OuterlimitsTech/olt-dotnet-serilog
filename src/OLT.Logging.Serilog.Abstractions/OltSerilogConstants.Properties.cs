namespace OLT.Logging.Serilog
{
    public static partial class OltSerilogConstants
    {
        /// <summary>
        /// Properties used for Serilog
        /// </summary>
        public static class Properties
        {
            public const string Username = "Username";
            public const string DbUsername = "DbUsername";
            public const string EventType = "OltEventType";
            public const string Environment = "Environment";

            [Obsolete("Removing in 10.x")]
            public const string DebuggerAttached = "DebuggerAttached";
            public const string Application = "Application";

            public static class AspNetCore
            {
                public const string AppRequestUid = "AppRequestUid";
                public const string RequestHeaders = "RequestHeaders";
                public const string ResponseHeaders = "ResponseHeaders";
                public const string RequestBody = "RequestBody";
                public const string ResponseBody = "ResponseBody";
                public const string RequestUri = "RequestUri";
            }

            public static class NgxMessage
            {
                public const string MessageAsJson = "ngx-message-json";
            }
        }



    }

}
