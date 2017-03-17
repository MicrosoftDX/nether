// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

using Nether.Common.DependencyInjection;
using Nether.Data.Analytics;
using Nether.Data.EntityFramework.Analytics;
using Nether.Data.InMemory.Analytics;
using Nether.Data.Sql.Analytics;
using Nether.Web.Features.Analytics.Models.Endpoint;
using Nether.Web.Utilities;

namespace Nether.Web.Features.Analytics
{
    public static class AnalyticsServiceExtensions
    {
        public static IServiceCollection AddAnalyticsServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            NetherServiceSwitchSettings serviceSwitches
            )
        {
            bool enabled = configuration.GetValue<bool>("Analytics:Enabled");
            if (!enabled)
            {
                logger.LogInformation("Analytics service not enabled");
                return services;
            }
            logger.LogInformation("Configuring Analytics service");
            serviceSwitches.AddServiceSwitch("Analytics", true);


            services.AddEndpointInfo(configuration, logger, "Analytics:EventHub");
            ConfigureAnalyticsStore(services, configuration, logger);

            return services;
        }

        private static IServiceCollection AddEndpointInfo(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            string key)
        {
            var endpointInfo = GetEndpointInfo(configuration.GetSection(key));

            logger.LogInformation("Analytics:EventHub: using resource '{0}'", endpointInfo.Resource);

            services.AddSingleton(endpointInfo);
            return services;
        }
        public static EndpointInfo GetEndpointInfo(IConfiguration configuration)
        {
            return new EndpointInfo
            {
                KeyName = configuration["KeyName"],
                AccessKey = configuration["AccessKey"],
                Resource = configuration["Resource"],
                Ttl = TimeSpan.Parse(configuration["Ttl"])
            };
        }



        private static void ConfigureAnalyticsStore(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            if (configuration.Exists("Analytics:Store:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["Analytics:Store:wellknown"];
                var scopedConfiguration = configuration.GetSection("Analytics:Store:properties");
                switch (wellKnownType)
                {
                    case "in-memory":
                        logger.LogInformation("Analytics:Store: using 'in-memory' store");
                        services.AddTransient<IAnalyticsStore, EntityFrameworkAnalyticsStore>();
                        services.AddTransient<AnalyticsContextBase, InMemoryAnalyticsContext>();
                        break;
                    case "sql":
                        logger.LogInformation("Analytics:Store: using 'Sql' store");
                        string connectionString = scopedConfiguration["ConnectionString"];
                        services.AddTransient<IAnalyticsStore, EntityFrameworkAnalyticsStore>();
                        // Add AnalyticsContextOptions to configure for SQL Server
                        services.AddSingleton(new SqlAnalyticsContextOptions { ConnectionString = connectionString });
                        services.AddTransient<AnalyticsContextBase, SqlAnalyticsContext>();
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for Analytics:Store: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<IAnalyticsStore>(configuration, logger, "Analytics:Store");
            }
        }

        // TODO - look at abstracting this behind a "UseIdentity" method or similar
        public static void InitializeAnalyticsStore(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
        {
            var serviceSwitchSettings = app.ApplicationServices.GetRequiredService<NetherServiceSwitchSettings>();
            if (!serviceSwitchSettings.IsServiceEnabled("Analytics"))
            {
                return;
            }

            var wellKnownType = configuration["Analytics:Store:wellknown"];
            if (wellKnownType == "sql")
            {
                logger.LogInformation("Run Migrations for SqlAnalyticsContext");
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = (SqlAnalyticsContext)serviceScope.ServiceProvider.GetRequiredService<AnalyticsContextBase>();
                    context.Database.Migrate();
                }
            }
        }
    }
}