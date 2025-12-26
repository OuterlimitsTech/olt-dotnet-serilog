namespace OLT.Logging.Serilog
{
    /// <summary>
    /// ngx-logger <see href="https://github.com/dbfannin/ngx-logger/blob/master/src/lib/types/logger-level.enum.ts"/> Log Levels
    /// </summary>
    public enum OltNgxLoggerLevel
    {
        Trace = 0,
        Debug,
        Information,
        Log,
        Warning,
        Error,
        Fatal,
        Off
    }
}