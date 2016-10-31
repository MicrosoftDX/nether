// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using Nether.Data.MongoDB.Leaderboard;

namespace Nether.Web.Features.Leaderboard
{
    public static class LeaderboardServiceExtensions
    {
        public static IServiceCollection AddLeaderboardServices(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO - look at what can be extracted to generalise this
            if (configuration.Exists("LeaderboardStore:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["LeaderboardStore:wellknown"];
                switch (wellKnownType)
                {
                    case "mongo":
                        var scopedConfiguration = configuration.GetSection("LeaderboardStore:properties");
                        string connectionString = scopedConfiguration["ConnectionString"];
                        string databaseName = scopedConfiguration["DatabaseName"];

                        services.AddTransient<ILeaderboardStore>(serviceProvider =>
                        {
                            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                            // TODO - look at encapsulating the connection info and registering that so that we can just register the type without the factory
                            return new MongoDBLeaderboardStore(connectionString, databaseName, loggerFactory);
                        });
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for LeaderboardStore: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<ILeaderboardStore>(configuration, "LeaderboardStore");
            }
            return services;
        }
    }
}