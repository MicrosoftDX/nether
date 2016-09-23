using Microsoft.Extensions.Configuration;
using Nether.Common.DependencyInjection;
using Nether.Integration.Analytics;

namespace Nether.Integration.Default.Analytics
{
    public class AnalyticsIntegrationNullClientConfigurationFactory : IDependencyFactory<IAnalyticsIntegrationClient>
    {
        public IAnalyticsIntegrationClient CreateInstance(IConfiguration configuration)
        {
            return new AnalyticsIntegrationNullClient();
        }
    }
}