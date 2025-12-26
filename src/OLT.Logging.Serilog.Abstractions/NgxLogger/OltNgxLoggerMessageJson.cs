namespace OLT.Logging.Serilog
{
    /// <summary>
    /// ngx-logger <see href="https://www.npmjs.com/package/ngx-logger"/> Log JSON model
    /// </summary>
    public class OltNgxLoggerMessageJson
    {
        public virtual string? Message { get; set; }
        public virtual List<List<OltNgxLoggerDetailJson>> Additional { get; set; } = new List<List<OltNgxLoggerDetailJson>>();
        public virtual OltNgxLoggerLevel? Level { get; set; }
        public virtual DateTimeOffset? Timestamp { get; set; }
        public virtual string? FileName { get; set; }
        public virtual int? LineNumber { get; set; }
        public virtual int? ColumnNumber { get; set; }
        public virtual bool IsError => Level.GetValueOrDefault(OltNgxLoggerLevel.Information) == OltNgxLoggerLevel.Fatal || Level.GetValueOrDefault(OltNgxLoggerLevel.Information) == OltNgxLoggerLevel.Error;

        public virtual string GetUsername()
        {
            return Additional.FirstOrDefault()?.FirstOrDefault()?.User ?? "Unknown";
        }

        public string? FormatMessage()
        {
            return ToException().Message;
        }

        public virtual Exception ToException()
        {
            var detail = Additional.FirstOrDefault()?.FirstOrDefault();
            var ex = detail != null ? detail.ToException() : new ApplicationException(Message);
            ex.Data.Add("Username", GetUsername());
            ex.Data.Add("Level", Level?.ToString());
            ex.Data.Add("FileName", FileName);
            ex.Data.Add("LineNumber", LineNumber);
            ex.Data.Add("ColumnNumber", ColumnNumber);            
            ex.Data.Add("Timestamp", Timestamp?.ToString(OltSerilogConstants.FormatString.ISO8601));
            return ex;
        }

        
    }


}