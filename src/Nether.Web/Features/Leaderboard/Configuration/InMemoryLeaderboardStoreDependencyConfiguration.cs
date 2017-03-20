using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using Nether.Data.EntityFramework.Leaderboard;
using Nether.Data.InMemory.Leaderboard;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class InMemoryLeaderboardStoreDependencyConfiguration : IDependencyConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // configure store and dependencies
            services.AddTransient<LeaderboardContextBase, InMemoryLeaderboardContext>();
            services.AddTransient<ILeaderboardStore, EntityFrameworkLeaderboardStore>();
        }
    }
}
