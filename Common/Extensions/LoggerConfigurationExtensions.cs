using Serilog;
using Serilog.Configuration;
using Services.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common.Extensions
{
    /// <summary>
    /// Class containing extension methods to <see cref="LoggerConfiguration"/>, configuring an
    /// enricher that extracts the current activity's trace and span IDs and then adds them
    /// as properties to the log event. 
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "This static class is not testable.")]
    public static class LoggerConfigurationExtensions
    {
        /// <summary>
        /// If there is an active, current activity, the Trace ID and Span ID
        /// are extracted and added to the log event as traceId and spanId 
        /// properties.
        ///
        /// This enricher is designed to work with the Serilog sink.
        /// </summary>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration WithDatadogTraceIdAndSpanId(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }

            return enrichmentConfiguration.With<DatadogTraceIdEnricher>();
        }
    }
}
