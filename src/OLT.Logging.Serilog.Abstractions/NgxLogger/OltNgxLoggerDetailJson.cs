using System;
using System.Collections.Generic;

namespace OLT.Logging.Serilog
{
    /// <summary>
    /// ngx-logger <see href="https://www.npmjs.com/package/ngx-logger"/> Log Detail JSON model
    /// </summary>
    /// <remarks>
    /// Child JSON model array for <see cref="OltNgxLoggerMessageJson.Additional"/>
    /// </remarks>
    public class OltNgxLoggerDetailJson
    {        
        public virtual string? Name { get; set; }
        public virtual string? AppId { get; set; }
        public virtual string? User { get; set; }
        public virtual long? Time { get; set; }
        public virtual string? Id { get; set; }
        public virtual string? Url { get; set; }
        public virtual int? Status { get; set; }
        public virtual string? Message { get; set; }
        public virtual List<OltNgxLoggerStackJson>? Stack { get; set; }

        public ApplicationException ToException()
        {
            var ex = new ApplicationException(Message)
            {
                Source = Id
            };

            ex.Data.Add("Name", Name);
            ex.Data.Add("AppId", AppId);
            ex.Data.Add("User", User);
            if (Time.HasValue)
            {
                var dt = DateTimeOffset.FromUnixTimeMilliseconds(Time.Value);
                ex.Data.Add("Time", dt.ToString(OltSerilogConstants.FormatString.ISO8601));
            }
            else
            {
                ex.Data.Add("Time", null);
            }
            ex.Data.Add("Url", Url);
            ex.Data.Add("Status", Status);            
            if (Stack?.Count > 0)
            {
                ex.Data.Add("Stack", Stack.FormatStack());
            }


            return ex;
        }



    }


}