using System;
using Microsoft.Extensions.Configuration;
using Nether.Common.DependencyInjection;
using Nether.Integration.Analytics;
using Microsoft.Extensions.DependencyInjection;

namespace Nether.Integration.Default.Analytics
{
    public class AnalyticsIntegrationClientConfigurationFactory : IDependencyFactory<IAnalyticsIntegrationClient>
    {
        public IAnalyticsIntegrationClient CreateInstance(IServiceProvider services)
        {
            var configuration = services.GetRequiredService<IConfiguration>().GetSection("AnalyticsIntegrationClient:properties");
            var analyticsBaseUrl = configuration["AnalyticsBaseUrl"];
            return new AnalyticsIntegrationClient(analyticsBaseUrl);
        }
    }
}