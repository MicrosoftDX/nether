// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using Nether.Data.MongoDB.Leaderboard;
using Nether.Data.Sql.Leaderboard;
using Nether.Integration.Analytics;
using Nether.Integration.Default.Analytics;
using Nether.Web.Features.Leaderboard.Configuration;
using System.Collections.Generic;

namespace Nether.Web.Features.Leaderboard
{
    public static class LeaderboardServiceExtensions
    {
        public static IServiceCollection AddLeaderboardServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddLeaderboardStore(services, configuration);

            AddAnalyticsIntegrationClient(services, configuration);

            return services;
        }

        private static void AddAnalyticsIntegrationClient(IServiceCollection services, IConfiguration configuration)
        {
            // TODO - look at what can be extracted to generalise this
            if (configuration.Exists("Leaderboard:AnalyticsIntegrationClient:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["Leaderboard:AnalyticsIntegrationClient:wellknown"];
                switch (wellKnownType)
                {
                    case "null":
                        services.AddSingleton<IAnalyticsIntegrationClient, AnalyticsIntegrationNullClient>();
                        break;
                    case "default":
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
                services.AddServiceFromConfiguration<IAnalyticsIntegrationClient>(configuration, "Leaderboard:AnalyticsIntegrationClient");
            }
        }

        private static void AddLeaderboardStore(IServiceCollection services, IConfiguration configuration)
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
                    case "mongo":
                        connectionString = scopedConfiguration["ConnectionString"];
                        string databaseName = scopedConfiguration["DatabaseName"];

                        services.AddTransient<ILeaderboardStore>(serviceProvider =>
                        {
                            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                            // TODO - look at encapsulating the connection info and registering that so that we can just register the type without the factory
                            return new MongoDBLeaderboardStore(connectionString, databaseName, loggerFactory);
                        });
                        break;
                    case "sql":
                        connectionString = scopedConfiguration["ConnectionString"];
                        services.AddTransient<ILeaderboardStore>(serviceProvider =>
                        {
                            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                            return new SqlLeaderboardStore(connectionString, loggerFactory);
                        });
                        services.AddTransient<ILeaderboardConfiguration>(ServiceProviderServiceExtensions =>
                        {
                            return new LeaderboardConfiguration(GetLeaderboardconfiguraion(configuration.GetSection("Leaderboard:Leaderboards").GetChildren()));
                        });
                        break;                        
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for Leaderboard:Store: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<ILeaderboardStore>(configuration, "Leaderboard:Store");
            }
        }

        private static Dictionary<string, LeaderboardConfig> GetLeaderboardconfiguraion(IEnumerable<IConfigurationSection> enumerable)
        {
            Dictionary<string, LeaderboardConfig> leaderboards = new Dictionary<string, LeaderboardConfig>();
            // go over all leaderboards under "Leaderboard:Leaderboards"
            foreach (var config in enumerable)
            {
                string name = config["Name"];

                leaderboards.Add(name, new LeaderboardConfig
                {
                    Name = name,
                    Type = (LeaderboardType)Enum.Parse(typeof(LeaderboardType), config["Type"]),
                    Radius = config["Radius"] != null? int.Parse(config["Radius"]) : 0,
                    Top = config["Top"] != null ? int.Parse(config["Top"]) : 0,
                });
            }

            return leaderboards;
        }        
       
    }
}