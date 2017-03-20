using Nether.Common.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class NullAnalyticsIntegrationClientDependencyConfiguration : IDependencyConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddSingleton<IAnalyticsIntegrationClient, AnalyticsIntegrationNullClient>();
        }
    }
}
