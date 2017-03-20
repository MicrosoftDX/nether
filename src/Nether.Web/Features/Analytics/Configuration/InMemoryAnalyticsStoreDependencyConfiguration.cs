using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.Analytics;
using Nether.Data.EntityFramework.Analytics;
using Nether.Data.InMemory.Analytics;

namespace Nether.Web.Features.Analytics.Configuration
{
    public class InMemoryAnalyticsStoreDependencyConfiguration : IDependencyConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // configure store and dependencies
            services.AddTransient<AnalyticsContextBase, InMemoryAnalyticsContext>();
            services.AddTransient<IAnalyticsStore, EntityFrameworkAnalyticsStore>();
        }
    }
}
