namespace OLT.Logging.Serilog
{
    public static class OltNgxLoggerStackExtensions
    {

        public static string FormatStack(this OltNgxLoggerStackJson value)
        {
            var list = new List<string>
            {
                $"Column Number: {value.ColumnNumber}",
                $"Line Number: {value.LineNumber}",
                $"FileName: {value.FileName}",
                $"FunctionName: {value.FunctionName}",
                $"Source: {value.Source}",
            };
            return string.Join(Environment.NewLine, list);
        }

        public static string FormatStack(this List<OltNgxLoggerStackJson> stack)
        {
            return string.Join($"----------------------------{Environment.NewLine}", stack.Select(s => $"{FormatStack(s)}{Environment.NewLine}").ToList());
        }
    }
}
