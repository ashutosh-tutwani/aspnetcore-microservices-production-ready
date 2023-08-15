using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Services.Common.Logging
{
    /// <summary>
    /// This class implements the ILogEventEnricher interface for 
    /// Serilog enrichers. If there is an active, current activity, 
    /// the Trace ID and Span ID are extracted, converted to 64-unsigned 
    /// int representations, and then added to the log event as 
    /// traceId and spanId properties.
    ///
    /// Although this enricher may be useful in other contexts, it is 
    /// designed to work with the Serilog sink, which inserts 
    /// these properties into the appropriate LogRecord fields.
    /// NOTE: Datadog specific implementation. To be removed once Open Telemetry Logs are used.
    /// </summary>
    internal class DatadogTraceIdEnricher : ILogEventEnricher
    {
        /// <summary>
        /// Property name for the trace ID extracted from the current activity.
        /// </summary>
        internal const string DD_TRACE_ID_PROPERTY_NAME = "dd.trace_id";
        internal const string TRACE_ID_PROPERTY_NAME = "trace_id";

        /// <summary>
        /// Property name for the span ID extracted from the current activity.
        /// </summary>
        internal const string SPAN_ID_PROPERTY_NAME = "span_id";
        internal const string DD_SPAN_ID_PROPERTY_NAME = "dd.span_id";

        /// <summary>
        /// Creates a new TraceIdEnricher instance.
        /// </summary>
        public DatadogTraceIdEnricher() { }

        /// <summary>
        /// Implements the `ILogEventEnricher` interface, adding the trace
        /// and span IDs from the current activity to the LogEvent.
        /// </summary>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string? strTraceId = Activity.Current?.TraceId.ToString();
            if (strTraceId != null)
            {
                string ddTraceId = Convert.ToUInt64(strTraceId.Substring(16), 16).ToString();
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(TRACE_ID_PROPERTY_NAME, strTraceId));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(DD_TRACE_ID_PROPERTY_NAME, ddTraceId));
            }

            string? strSpanId = Activity.Current?.SpanId.ToString();
            if (strSpanId != null)
            {
                string ddSpanId = Convert.ToUInt64(strSpanId, 16).ToString();
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(SPAN_ID_PROPERTY_NAME, strSpanId));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(DD_SPAN_ID_PROPERTY_NAME, ddSpanId));
            }
        }
    }
}
