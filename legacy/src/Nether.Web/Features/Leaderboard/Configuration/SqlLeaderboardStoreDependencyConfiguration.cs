// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    public class SqlLeaderboardStoreDependencyConfiguration : DependencyConfiguration, IDependencyInitializer<ILeaderboardStore>
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure store and dependencies
            context.Services.AddSingleton(context.ScopedConfiguration.Get<SqlLeaderboardContextOptions>());

            context.Services.AddTransient<LeaderboardContextBase, SqlLeaderboardContext>();
            context.Services.AddTransient<ILeaderboardStore, EntityFrameworkLeaderboardStore>();

            // configure type to perform migrations
            context.Services.AddSingleton<IDependencyInitializer<ILeaderboardStore>>(this);
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
