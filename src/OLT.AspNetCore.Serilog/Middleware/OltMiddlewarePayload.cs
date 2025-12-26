using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OLT.Core;
using Serilog;
using Serilog.Events;

namespace OLT.Logging.Serilog
{
    public class OltMiddlewarePayload : IMiddleware
    {
        private const string ContentType = "application/json";
        private readonly OltSerilogOptions _options;

        public OltMiddlewarePayload(IOptions<OltSerilogOptions> options)
        {
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Guid uid = Guid.NewGuid();
            var requestUri = $"{context.Request.Scheme}//{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            var requestBodyText = await FormatRequestAsync(context.Request, _options.BodyPayloadLimit);
            var logLevel = LogEventLevel.Debug;

            await using MemoryStream responseBodyStream = new MemoryStream();
            var originalResponseBodyReference = context.Response.Body;
            context.Response.Body = responseBodyStream;

            try
            {
                await next(context);
            }
            catch (OltBadRequestException badRequestException)
            {
                var msg = new OltErrorHttpSerilog { ErrorUid = uid, Message = badRequestException.Message };
                context.Response.ContentType = ContentType;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logLevel = LogEventLevel.Warning;
                await context.Response.WriteAsync(msg.ToJson());
            }
            catch (OltValidationException validationException)
            {
                var msg = new OltErrorHttpSerilog
                {
                    ErrorUid = uid,
                    Message = validationException.Message,
                    Errors = validationException.Results.Where(p => p.Message != null).Select(s => s.Message ?? string.Empty).ToList()
                };
                context.Response.ContentType = ContentType;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logLevel = LogEventLevel.Warning;
                await context.Response.WriteAsync(msg.ToJson());
            }
            catch (OltRecordNotFoundException recordNotFoundException)
            {
                var msg = new OltErrorHttpSerilog { ErrorUid = uid, Message = recordNotFoundException.Message };
                context.Response.ContentType = ContentType;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logLevel = LogEventLevel.Warning;
                await context.Response.WriteAsync(msg.ToJson());
            }
            catch (Exception exception)
            {
                logLevel = LogEventLevel.Error;
                var msg = FormatServerError(context, exception, uid);
                await context.Response.WriteAsync(msg.ToJson());
            }

            var responseBodyText = await FormatResponseAsync(context.Response, _options.BodyPayloadLimit);
            var logger = BuildLogger(context, uid, requestUri, requestBodyText, responseBodyText);
            logger.Write(logLevel, OltSerilogConstants.Templates.AspNetCore.Payload, uid, context.Request.Method, context.Request.Path, context.Response.StatusCode);

            await responseBodyStream.CopyToAsync(originalResponseBodyReference);
        }

        private static ILogger BuildLogger(HttpContext context, Guid uid, string requestUri, string? requestBodyText, string? responseBodyText)
        {
            return Log
                .ForContext(OltSerilogConstants.Properties.AspNetCore.AppRequestUid, uid)
                .ForContext(OltSerilogConstants.Properties.AspNetCore.RequestHeaders, context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext(OltSerilogConstants.Properties.AspNetCore.ResponseHeaders, context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext(OltSerilogConstants.Properties.AspNetCore.RequestBody, requestBodyText)
                .ForContext(OltSerilogConstants.Properties.AspNetCore.ResponseBody, responseBodyText)
                .ForContext(OltSerilogConstants.Properties.AspNetCore.RequestUri, requestUri);
        }

        private OltErrorHttpSerilog FormatServerError(HttpContext context, Exception exception, Guid uid)
        {
            Log.ForContext(OltSerilogConstants.Properties.AspNetCore.AppRequestUid, uid)
                .Error(exception, OltSerilogConstants.Templates.AspNetCore.ServerError, uid, exception.Message);
            context.Response.ContentType = ContentType;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var result = new OltErrorHttpSerilog { ErrorUid = uid, Message = _options.ErrorMessage };
            if (_options.ShowExceptionDetails)
            {
                result.Errors = GetInnerExceptions(exception).Select(s => s.Message).ToList();
            }
            return result;
        }

        private static IEnumerable<Exception> GetInnerExceptions(Exception ex)
        {
            var innerException = ex;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }

        private static async Task<string?> FormatRequestAsync(HttpRequest request, double bodyPayloadLimit)
        {
            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();
            var reader = new StreamReader(request.Body);
            string body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            if (string.IsNullOrWhiteSpace(body))
            {
                return null;
            }

            var byteCount = System.Text.ASCIIEncoding.ASCII.GetByteCount(body);
            var mb = ConvertBytesToMegabytes(byteCount);
            if (mb > bodyPayloadLimit)
            {
                var size = ToSizeString(byteCount);
                return $"Truncated [{size}]";
            }

            return body;
        }

        private static async Task<string?> FormatResponseAsync(HttpResponse response, double bodyPayloadLimit)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            if (string.IsNullOrWhiteSpace(body))
            {
                return null;
            }

            var byteCount = System.Text.ASCIIEncoding.ASCII.GetByteCount(body);
            var mb = ConvertBytesToMegabytes(byteCount);
            if (mb > bodyPayloadLimit)
            {
                var size = ToSizeString(byteCount);
                return $"Truncated [{size}]";
            }

            return body;
        }



        static double ConvertBytesToMegabytes(int bytes)
        {
            var mb = 1024f * 1024f;
            return Convert.ToDouble(bytes) / mb;
            //return (bytes / 1024f) / 1024f;
        }

        private static string ToSizeString(long value)
        {
            long KB = 1024;
            long MB = KB * 1024;
            long GB = MB * 1024;
            long TB = GB * 1024;
            double size = value;
            if (value >= TB)
            {
                size = Math.Round((double)value / TB, 2);
                return $"{size} TB";
            }
            else if (value >= GB)
            {
                size = Math.Round((double)value / GB, 2);
                return $"{size} GB";
            }
            else if (value >= MB)
            {
                size = Math.Round((double)value / MB, 2);
                return $"{size} MB";
            }
            else if (value >= KB)
            {
                size = Math.Round((double)value / KB, 2);
                return $"{size} KB";
            }
            else
            {
                return $"{size} Bytes";
            }
        }
    }
}
