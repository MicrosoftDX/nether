using Nether.Common.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class HttpAnalyticsIntegrationClientDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            string baseUrl = context.ScopedConfiguration["AnalyticsBaseUrl"];

            context.Services.AddTransient<IAnalyticsIntegrationClient>(serviceProvider =>
            {
                return new AnalyticsIntegrationHttpClient(baseUrl);
            });
        }
    }
}
