using System;
using System.Collections.Generic;
using System.Text;

namespace OLT.Constants
{
    public static class OltSerilogMsSqlConstants
    {
        public static class Table
        {
            public const string Name = "Log";
            public const string Schema = "dbo";
        }

        public static class ColumnNames
        {
            public const string Id = "LogId";
            public const string Application = "Application";
            public const string RequestPath = "RequestPath";
            public const string Source = "Source";
        }

    }
}
