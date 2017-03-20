// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

using Nether.Common.DependencyInjection;
using Nether.Data.PlayerManagement;
using Nether.Data.EntityFramework.PlayerManagement;
using Nether.Data.InMemory.PlayerManagement;
using Nether.Data.MongoDB.PlayerManagement;
using Nether.Data.Sql.PlayerManagement;
using Nether.Web.Features.PlayerManagement.Configuration
using Nether.Web.Utilities;
using System.Collections.Generic;

namespace Nether.Web.Features.PlayerManagement
{
    public static class PlayerManagementServiceExtensions
    {
        private static Dictionary<string, Type> _wellKnownStoreTypes = new Dictionary<string, Type>
            {
                {"in-memory", typeof(InMemoryPlayerManagementStoreDependencyConfiguration) },
                {"sql", typeof(SqlPlayerManagementStoreDependencyConfiguration) },
                {"mongo", typeof(SqlPlayerManagementStoreDependencyConfiguration) },
            };

        public static IServiceCollection AddPlayerManagementServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            NetherServiceSwitchSettings serviceSwitches
            )
        {
            bool enabled = configuration.GetValue<bool>("PlayerManagement:Enabled");
            if (!enabled)
            {
                logger.LogInformation("PlayerManagement service not enabled");
                return services;
            }
            logger.LogInformation("Configuring PlayerManagement service");
            serviceSwitches.AddServiceSwitch("PlayerManagement", true);

            services.AddServiceFromConfiguration("PlayerManagement:Store", _wellKnownStoreTypes, configuration, logger);

            return services;
        }
        // TODO - look at abstracting this behind a "UsePlayerManagement" method or similar
        public static void InitializePlayerManagementStore(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
        {
            var serviceSwitchSettings = app.ApplicationServices.GetRequiredService<NetherServiceSwitchSettings>();
            if (!serviceSwitchSettings.IsServiceEnabled("PlayerManagement"))
            {
                return;
            }

            var wellKnownType = configuration["PlayerManagement:Store:wellknown"];
            if (wellKnownType == "sql")
            {
                logger.LogInformation("Run Migrations for SqlPlayerManagementContext");
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = (SqlPlayerManagementContext)serviceScope.ServiceProvider.GetRequiredService<PlayerManagementContextBase>();
                    context.Database.Migrate();
                }
            }
        }
    }
}
