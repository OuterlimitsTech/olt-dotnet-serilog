using System;
using System.Text;
using Murmur;
using Serilog.Core;
using Serilog.Events;

namespace OLT.Logging.Serilog.Enricher
{
    public class OltEventTypeEnricher : ILogEventEnricher
    {
        public const string PropertyName = OltSerilogConstants.Properties.EventType;

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            Murmur32 murmur = MurmurHash.Create32();
            byte[] bytes = Encoding.UTF8.GetBytes(logEvent.MessageTemplate.Text);
            byte[] hash = murmur.ComputeHash(bytes);
            string hexadecimalHash = BitConverter.ToString(hash).Replace("-", "");
            LogEventProperty eventId = propertyFactory.CreateProperty(PropertyName, hexadecimalHash);
            logEvent.AddPropertyIfAbsent(eventId);
        }
    }
}