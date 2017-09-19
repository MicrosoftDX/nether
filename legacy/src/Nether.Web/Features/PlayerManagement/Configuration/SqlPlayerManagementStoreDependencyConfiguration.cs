// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nether.Common.DependencyInjection;
using Nether.Data.EntityFramework.PlayerManagement;
using Nether.Data.Sql.PlayerManagement;
using Nether.Data.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement.Configuration
{
    public class SqlPlayerManagementStoreDependencyConfiguration : DependencyConfiguration, IDependencyInitializer<IPlayerManagementStore>
    {
        protected override void OnConfigureServices(DependencyConfigurationContext context)
        {
            // configure store and dependencies
            context.Services.AddSingleton(context.ScopedConfiguration.Get<SqlPlayerManagementContextOptions>());

            context.Services.AddTransient<PlayerManagementContextBase, SqlPlayerManagementContext>();
            context.Services.AddTransient<IPlayerManagementStore, EntityFrameworkPlayerManagementStore>();

            // configure type to perform migrations
            context.Services.AddSingleton<IDependencyInitializer<IPlayerManagementStore>>(this);
        }

        public IApplicationBuilder Use(IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<SqlPlayerManagementStoreDependencyConfiguration>>();
            logger.LogInformation("Run Migrations for SqlPlayerManagementContext");
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = (SqlPlayerManagementContext)serviceScope.ServiceProvider.GetRequiredService<PlayerManagementContextBase>();
                context.Database.Migrate();
            }

            return app;
        }
    }
}
