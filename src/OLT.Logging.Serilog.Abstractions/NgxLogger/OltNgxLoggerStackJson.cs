namespace OLT.Logging.Serilog
{
    /// <summary>
    /// ngx-logger <see href="https://www.npmjs.com/package/ngx-logger"/> Log Stack JSON model
    /// </summary>
    /// <remarks>
    /// Child JSON model for <see cref="OltNgxLoggerDetailJson.Stack"/>
    /// </remarks>
    public class OltNgxLoggerStackJson
    {
        public virtual int? ColumnNumber { get; set; }
        public virtual int? LineNumber { get; set; }
        public virtual string? FileName { get; set; }
        public virtual string? FunctionName { get; set; }
        public virtual string? Source { get; set; }
    }
}
