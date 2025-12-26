namespace OLT.Logging.Serilog
{
    public static partial class OltSerilogConstants
    {
        public static class Templates
        {
            public static class AspNetCore
            {
                public const string ServerError = "{AppRequestUid}:{Message}";
                public const string Payload = "{AppRequestUid}:APP PAYLOAD LOG {RequestMethod} {RequestPath} {statusCode}";
            }

            public const string DefaultOutput =
                "[{Timestamp:HH:mm:ss} {Level:u3}] {OltEventType:x8} {Message:lj}{NewLine}{Exception}";



            public static class Email
            {
                public static string DefaultEmail => Environment.NewLine +
                                                     Environment.NewLine +
                                                     DefaultOutput +
                                                     Environment.NewLine +
                                                     Environment.NewLine;

                public const string DefaultSubject =
                    "APPLICATION {Level} on {Application} {Environment} Environment occurred at {Timestamp}";
            }

            public static class NgxMessage
            {
                public const string Template = "ngx-message: {ngx-message}";
            }
        }



    }

}
