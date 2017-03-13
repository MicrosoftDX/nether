// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Common.ApplicationPerformanceMonitoring
{
    //
    public class ApplicationInsightsMonitor : IApplicationPerformanceMonitor
    {
        private readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsMonitor(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void LogError(Exception exception, string message = null, IDictionary<string, string> properties = null)
        {
            var telemetry = new ExceptionTelemetry(exception);
            AddProperties(telemetry, message, properties);
            _telemetryClient.TrackException(telemetry);
        }

        public void LogEvent(string eventName, string message = null, IDictionary<string, string> properties = null)
        {
            var telemetry = new EventTelemetry(eventName);
            AddProperties(telemetry, message, properties);
            _telemetryClient.TrackEvent(telemetry);
        }

        private static void AddProperties(ISupportProperties eventTelemetry, string message, IDictionary<string, string> properties)
        {
            if (message != null)
            {
                eventTelemetry.Properties.Add("_Message", message);
            }
            if (properties != null)
            {
                foreach (var item in properties)
                {
                    eventTelemetry.Properties.Add(item);
                }
            }
        }
    }
}
