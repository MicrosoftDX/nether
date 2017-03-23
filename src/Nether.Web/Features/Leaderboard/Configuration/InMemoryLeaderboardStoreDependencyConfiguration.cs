using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using Nether.Data.EntityFramework.Leaderboard;
using Nether.Data.InMemory.Leaderboard;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class InMemoryLeaderboardStoreDependencyConfiguration : DependencyConfiguration
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure store and dependencies
            context.Services.AddTransient<LeaderboardContextBase, InMemoryLeaderboardContext>();
            context.Services.AddTransient<ILeaderboardStore, EntityFrameworkLeaderboardStore>();
        }
    }
}
