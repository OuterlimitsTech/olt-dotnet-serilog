using Microsoft.AspNetCore.Http;
using OLT.Core;
using System.Threading.Tasks;
using LogContext = Serilog.Context.LogContext;

namespace OLT.Logging.Serilog
{

    public class OltMiddlewareSession : IMiddleware
    {
        private readonly IOltIdentity? _identity;

        public OltMiddlewareSession(IOltIdentity? identity = null)
        {
            _identity = identity;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (LogContext.PushProperty(OltSerilogConstants.Properties.Username, _identity?.Username))
            using (LogContext.PushProperty(OltSerilogConstants.Properties.DbUsername, _identity?.GetDbUsername()))
            {
                await next(context);
            }
            
        }
    }
}
