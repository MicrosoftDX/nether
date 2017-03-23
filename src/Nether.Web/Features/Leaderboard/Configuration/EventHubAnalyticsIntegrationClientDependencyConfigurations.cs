using Nether.Common.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class EventHubAnalyticsIntegrationClientDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            string eventHubConnectionString = context.ScopedConfiguration["EventHubConnectionString"];

            context.Services.AddSingleton<IAnalyticsIntegrationClient>(serviceProvider =>
            {
                return new AnalyticsIntegrationEventHubClient(eventHubConnectionString);
            });
        }
    }
}
