using Microsoft.Extensions.Configuration;
using Nether.Common.DependencyInjection;
using Nether.Integration.Analytics;

namespace Nether.Integration.Default.Analytics
{
    //TODO: Fix and perhaps remove once DI work has progressed
    public class AnalyticsIntegrationNullClientConfigurationFactory : IDependencyFactory<IAnalyticsIntegrationClient>
    {
        public IAnalyticsIntegrationClient CreateInstance(IConfiguration configuration)
        {
            return new AnalyticsIntegrationNullClient();
        }
    }
}