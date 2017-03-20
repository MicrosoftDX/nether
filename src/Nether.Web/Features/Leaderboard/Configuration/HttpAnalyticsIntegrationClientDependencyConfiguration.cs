using Nether.Common.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class HttpAnalyticsIntegrationClientDependencyConfiguration : IDependencyConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var scopedConfiguration = configuration.GetSection("Leaderboard:AnalyticsIntegrationClient:properties");
            string baseUrl = scopedConfiguration["AnalyticsBaseUrl"];

            services.AddTransient<IAnalyticsIntegrationClient>(serviceProvider =>
            {
                return new AnalyticsIntegrationHttpClient(baseUrl);
            });
        }
    }
}
