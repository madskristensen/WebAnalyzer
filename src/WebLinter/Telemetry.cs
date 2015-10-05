using System;
using Microsoft.ApplicationInsights;

namespace WebLinter
{
    /// <summary>
    /// Reports anonymous usage through ApplicationInsights
    /// </summary>
    public static class Telemetry
    {
        private static TelemetryClient _telemetry = GetAppInsightsClient();
        private const string TELEMETRY_KEY = "468caa30-c256-47c7-946d-1f0230ccec9d";

        /// <summary>Determines if telemetry should be reported.</summary>
        public static bool Enabled { get; set; } = true;

        private static TelemetryClient GetAppInsightsClient()
        {
            TelemetryClient client = new TelemetryClient();
            client.InstrumentationKey = TELEMETRY_KEY;
            client.Context.Component.Version = Constants.VERSION;
            client.Context.Session.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();

            return client;
        }

        /// <summary>Tracks an event to ApplicationInsights.</summary>
        public static void TrackEvent(string message)
        {
#if !DEBUG
            if (Enabled)
            {
                _telemetry.TrackEvent(message);
            }
#endif
        }

        /// <summary>Tracks any exception.</summary>
        public static void TrackException(Exception ex)
        {
#if !DEBUG
            if (Enabled)
            {
                var telex = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(ex);
                telex.HandledAt = Microsoft.ApplicationInsights.DataContracts.ExceptionHandledAt.UserCode;
                _telemetry.TrackException(telex);
            }
#endif
        }
    }
}
