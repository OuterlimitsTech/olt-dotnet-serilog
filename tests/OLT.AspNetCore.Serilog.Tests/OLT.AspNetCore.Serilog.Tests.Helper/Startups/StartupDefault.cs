using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace OLT.AspNetCore.Serilog.Tests
{
    [ExcludeFromCodeCoverage]
    public class StartupDefault : BaseStartup
    {
        public StartupDefault(IConfiguration configuration) : base(configuration) { }

    }
}
