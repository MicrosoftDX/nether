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
    public class EventHubAnalyticsIntegrationClientDependencyConfiguration : IDependencyConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var scopedConfiguration = configuration.GetSection("Leaderboard:AnalyticsIntegrationClient:properties");
            string eventHubConnectionString = scopedConfiguration["EventHubConnectionString"];

            services.AddSingleton<IAnalyticsIntegrationClient>(serviceProvider =>
            {
                return new AnalyticsIntegrationEventHubClient(eventHubConnectionString);
            });
        }
    }
}
