using Serilog;

namespace OLT.Core
{
    public interface IOltSerilogHostApplicationBuilder : IOltApplicationHostBuilder
    {
        LoggerConfiguration LoggerConfiguration { get; }
    }

}

