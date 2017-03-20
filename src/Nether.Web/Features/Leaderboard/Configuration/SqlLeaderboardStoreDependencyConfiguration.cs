using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.EntityFramework.Leaderboard;
using Nether.Data.Leaderboard;
using Nether.Data.Sql.Leaderboard;

namespace Nether.Web.Features.Leaderboard.Configuration
{
    public class SqlLeaderboardStoreDependencyConfiguration : IDependencyConfiguration, IDependencyInitializer<ILeaderboardStore>
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // configure store and dependencies
            var scopedConfiguration = configuration.GetSection("Leaderboard:Store:properties");
            services.AddSingleton(scopedConfiguration.Get<SqlLeaderboardContextOptions>());

            services.AddTransient<LeaderboardContextBase, SqlLeaderboardContext>();
            services.AddTransient<ILeaderboardStore, EntityFrameworkLeaderboardStore>();

            // configure type to perform migrations
            services.AddTransient<IDependencyInitializer<ILeaderboardStore>, SqlLeaderboardStoreDependencyConfiguration>();
        }

        public IApplicationBuilder Use(IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<SqlLeaderboardStoreDependencyConfiguration>>();
            logger.LogInformation("Run Migrations for SqlLeaderboardContext");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = (SqlLeaderboardContext)serviceScope.ServiceProvider.GetRequiredService<LeaderboardContextBase>();
                context.Database.Migrate();
            }

            return app;
        }
    }
}
