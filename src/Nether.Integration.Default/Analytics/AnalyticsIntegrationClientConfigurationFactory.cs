using Microsoft.Extensions.Configuration;
using Nether.Common.DependencyInjection;
using Nether.Integration.Analytics;

namespace Nether.Integration.Default.Analytics
{
    public class AnalyticsIntegrationClientConfigurationFactory : IDependencyFactory<IAnalyticsIntegrationClient>
    {
        public IAnalyticsIntegrationClient CreateInstance(IConfiguration configuration)
        {
            var analyticsBaseUrl = configuration["AnalyticsIntegrationClient:properties:AnalyticsBaseUrl"];
            return new AnalyticsIntegrationClient(analyticsBaseUrl);
        }
    }
}