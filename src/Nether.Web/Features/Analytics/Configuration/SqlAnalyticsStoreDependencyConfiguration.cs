// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.EntityFramework.Analytics;
using Nether.Data.Sql.Analytics;
using Nether.Data.Analytics;

namespace Nether.Web.Features.Analytics.Configuration
{
    public class SqlAnalyticsStoreDependencyConfiguration : DependencyConfiguration, IDependencyInitializer<IAnalyticsStore>
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure store and dependencies
            context.Services.AddSingleton(context.ScopedConfiguration.Get<SqlAnalyticsContextOptions>());

            context.Services.AddTransient<AnalyticsContextBase, SqlAnalyticsContext>();
            context.Services.AddTransient<IAnalyticsStore, EntityFrameworkAnalyticsStore>();

            // configure type to perform migrations
            context.Services.AddSingleton<IDependencyInitializer<IAnalyticsStore>>(this);
        }

        public IApplicationBuilder Use(IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<SqlAnalyticsStoreDependencyConfiguration>>();
            logger.LogInformation("Run Migrations for SqlPlayerManagementContext");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = (SqlAnalyticsContext)serviceScope.ServiceProvider.GetRequiredService<AnalyticsContextBase>();
                context.Database.Migrate();
            }

            return app;
        }
    }
}
