using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Common.ApplicationPerformanceMonitoring;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class NullMonitorDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            context.Services.AddSingleton<IApplicationPerformanceMonitor, NullMonitor>();
        }
    }
}
