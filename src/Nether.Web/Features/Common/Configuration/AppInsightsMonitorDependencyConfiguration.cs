using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Common.ApplicationPerformanceMonitoring;
using System;
using Microsoft.AspNetCore.Hosting;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class AppInsightsMonitorDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            var instrumentationKey = context.ScopedConfiguration["InstrumentationKey"];
            context.Logger.LogInformation("ApplicationPerformanceMonitor: using 'appinsights' client with InstrumentationKey '{0}'", instrumentationKey);
            context.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.DeveloperMode = context.HostingEnvironment.IsDevelopment();
                options.InstrumentationKey = instrumentationKey;
            });
            context.Services.AddTransient<IApplicationPerformanceMonitor, ApplicationInsightsMonitor>();
        }
    }
}
