// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using Nether.Data.EntityFramework.Leaderboard;
using Nether.Data.InMemory.Leaderboard;
using Nether.Data.Sql.Leaderboard;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;
using Nether.Web.Features.Leaderboard.Configuration;
using Nether.Web.Utilities;

namespace Nether.Web.Features.Leaderboard
{
    public static class LeaderboardServiceExtensions
    {
        public static IServiceCollection AddLeaderboardServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            NetherServiceSwitchSettings serviceSwitches
            )
        {
            bool enabled = configuration.GetValue<bool>("Leaderboard:Enabled");
            if (!enabled)
            {
                logger.LogInformation("Leaderboard service not enabled");
                return services;
            }
            logger.LogInformation("Configuring Leaderboard service");
            serviceSwitches.AddServiceSwitch("Leaderboard", true);

            AddLeaderboardStore(services, configuration, logger);

            AddAnalyticsIntegrationClient(services, configuration, logger);

            return services;
        }

        private static void AddAnalyticsIntegrationClient(
            IServiceCollection services,
            IConfiguration configuration,
            ILogger logger)
        {
            // TODO - look at what can be extracted to generalise this
            if (configuration.Exists("Leaderboard:AnalyticsIntegrationClient:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["Leaderboard:AnalyticsIntegrationClient:wellknown"];
                switch (wellKnownType)
                {
                    case "null":
                        logger.LogInformation("Leaderboard:AnalyticsIntegrationClient: using 'null' client");
                        services.AddSingleton<IAnalyticsIntegrationClient, AnalyticsIntegrationNullClient>();
                        break;
                    case "http":
                        {
                            logger.LogInformation("Leaderboard:AnalyticsIntegrationClient: using 'http' client");
                            var scopedConfiguration = configuration.GetSection("Leaderboard:AnalyticsIntegrationClient:properties");
                            string baseUrl = scopedConfiguration["AnalyticsBaseUrl"];

                            services.AddTransient<IAnalyticsIntegrationClient>(serviceProvider =>
                            {
                                return new AnalyticsIntegrationHttpClient(baseUrl);
                            });
                        }
                        break;
                    case "eventhub":
                        {
                            logger.LogInformation("Leaderboard:AnalyticsIntegrationClient: using 'eventhub' client");
                            var scopedConfiguration = configuration.GetSection("Leaderboard:AnalyticsIntegrationClient:properties");
                            string eventHubConnectionString = scopedConfiguration["EventHubConnectionString"];

                            services.AddSingleton<IAnalyticsIntegrationClient>(serviceProvider =>
                            {
                                return new AnalyticsIntegrationEventHubClient(eventHubConnectionString);
                            });
                        }
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for AnalyticsIntegrationClient: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<IAnalyticsIntegrationClient>(configuration, logger, "Leaderboard:AnalyticsIntegrationClient");
            }
        }

        private static void AddLeaderboardStore(
            IServiceCollection services,
            IConfiguration configuration,
            ILogger logger)
        {
            // TODO - look at what can be extracted to generalise this
            if (configuration.Exists("Leaderboard:Store:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["Leaderboard:Store:wellknown"];
                var scopedConfiguration = configuration.GetSection("Leaderboard:Store:properties");
                string connectionString;
                switch (wellKnownType)
                {
                    case "in-memory":
                        logger.LogInformation("Leaderboard:Store: using 'in-memory' store");
                        connectionString = scopedConfiguration["ConnectionString"];
                        services.AddTransient<LeaderboardContextBase, InMemoryLeaderboardContext>();
                        services.AddTransient<ILeaderboardStore, EntityFrameworkLeaderboardStore>();

                        break;
                    case "sql":
                        logger.LogInformation("Leaderboard:Store: using 'sql' store");
                        connectionString = scopedConfiguration["ConnectionString"];
                        services.AddSingleton(new SqlLeaderboardContextOptions { ConnectionString = connectionString });
                        services.AddTransient<LeaderboardContextBase, SqlLeaderboardContext>();
                        services.AddTransient<ILeaderboardStore, EntityFrameworkLeaderboardStore>();
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for Leaderboard:Store: '{wellKnownType}'");
                }
                services.AddTransient<ILeaderboardConfiguration>(serviceProvider =>
                {
                    return new LeaderboardConfiguration(GetLeaderboardConfiguration(configuration.GetSection("Leaderboard:Leaderboards")));
                });
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<ILeaderboardStore>(configuration, logger, "Leaderboard:Store");
            }
        }

        private static Dictionary<string, LeaderboardConfig> GetLeaderboardConfiguration(IConfigurationSection configuration)
        {
            Dictionary<string, LeaderboardConfig> leaderboards = new Dictionary<string, LeaderboardConfig>();
            // go over all leaderboards under "Leaderboard:Leaderboards"
            foreach (var config in configuration.GetChildren())
            {
                string name = config.Key;
                bool includeCurrentPlayer = bool.Parse(config["IncludeCurrentPlayer"] ?? "false");
                LeaderboardType type = (LeaderboardType)Enum.Parse(typeof(LeaderboardType), config["Type"]);
                LeaderboardConfig leaderboardConfig = new LeaderboardConfig
                {
                    Name = name,
                    Type = type,
                    IncludeCurrentPlayer = includeCurrentPlayer
                };

                switch (type)
                {
                    case LeaderboardType.Top:
                        string top = config["Top"];
                        if (top == null)
                        {
                            throw new Exception($"Leaderboard type Top must have Top value set. Leaderboard name: '{name}'");
                        }
                        else
                        {
                            leaderboardConfig.Top = int.Parse(top);
                        }
                        break;
                    case LeaderboardType.AroundMe:
                        string radius = config["Radius"];
                        if (radius == null)
                        {
                            throw new Exception($"Leaderboard type AroundMe must have Radius value set. Leaderboard name: '{name}'");
                        }
                        else
                        {
                            leaderboardConfig.Radius = int.Parse(radius);
                        }
                        break;
                    case LeaderboardType.All:
                        break;
                }

                leaderboards.Add(name, leaderboardConfig);
            }

            return leaderboards;
        }
        // TODO - look at abstracting this behind a "UseLeaderboard" method or similar
        public static void InitializeLeaderboardStore(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
        {
            var serviceSwitchSettings = app.ApplicationServices.GetRequiredService<NetherServiceSwitchSettings>();
            if (!serviceSwitchSettings.IsServiceEnabled("Leaderboard"))
            {
                return;
            }

            var wellKnownType = configuration["Leaderboard:Store:wellknown"];
            if (wellKnownType == "sql")
            {
                logger.LogInformation("Run Migrations for SqlLeaderboardContext");
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = (SqlLeaderboardContext)serviceScope.ServiceProvider.GetRequiredService<LeaderboardContextBase>();
                    context.Database.Migrate();
                }
            }
        }
    }
}