// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using Nether.Data.Sql.Leaderboard;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;
using Nether.Web.Features.Leaderboard.Configuration;
using System.Collections.Generic;

namespace Nether.Web.Features.Leaderboard
{
    public static class LeaderboardServiceExtensions
    {
        public static IServiceCollection AddLeaderboardServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger)
        {
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
                    case "default":
                        logger.LogInformation("Leaderboard:AnalyticsIntegrationClient: using 'default' client");
                        var scopedConfiguration = configuration.GetSection("Leaderboard:AnalyticsIntegrationClient:properties");
                        string baseUrl = scopedConfiguration["AnalyticsBaseUrl"];

                        services.AddTransient<IAnalyticsIntegrationClient>(serviceProvider =>
                        {
                            return new AnalyticsIntegrationClient(baseUrl);
                        });
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
                    case "sql":
                        logger.LogInformation("Leaderboard:Store: using 'Sql' store");
                        connectionString = scopedConfiguration["ConnectionString"];
                        services.AddSingleton(new SqlLeaderboardContextOptions { ConnectionString = connectionString });
                        services.AddTransient<LeaderboardContextBase, SqlLeaderboardContext>();
                        services.AddTransient<ILeaderboardStore, EntityFrameworkLeaderboardStore>();
                        services.AddTransient<ILeaderboardConfiguration>(ServiceProviderServiceExtensions =>
                        {
                            return new LeaderboardConfiguration(GetLeaderboardConfiguration(configuration.GetSection("Leaderboard:Leaderboards").GetChildren()));
                        });
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for Leaderboard:Store: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<ILeaderboardStore>(configuration, logger, "Leaderboard:Store");
            }
        }

        private static Dictionary<string, LeaderboardConfig> GetLeaderboardConfiguration(IEnumerable<IConfigurationSection> enumerable)
        {
            Dictionary<string, LeaderboardConfig> leaderboards = new Dictionary<string, LeaderboardConfig>();
            // go over all leaderboards under "Leaderboard:Leaderboards"
            foreach (var config in enumerable)
            {
                string name = config["Name"];
                LeaderboardType type = (LeaderboardType)Enum.Parse(typeof(LeaderboardType), config["Type"]);
                LeaderboardConfig leaderboardConfig = new LeaderboardConfig
                {
                    Name = name,
                    Type = type
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
    }
}