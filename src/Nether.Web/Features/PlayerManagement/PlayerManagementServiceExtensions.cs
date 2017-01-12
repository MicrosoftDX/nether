// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Nether.Common.DependencyInjection;
using Nether.Data.PlayerManagement;
using Nether.Data.MongoDB.PlayerManagement;
using Nether.Data.Sql.PlayerManagement;

namespace Nether.Web.Features.PlayerManagement
{
    public static class PlayerManagementServiceExtensions
    {
        public static IServiceCollection AddPlayerManagementServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger)
        {
            // TODO - look at what can be extracted to generalise this
            if (configuration.Exists("PlayerManagement:Store:wellKnown"))
            {
                // register using well-known type
                var wellKnownType = configuration["PlayerManagement:Store:wellknown"];
                var scopedConfiguration = configuration.GetSection("PlayerManagement:Store:properties");
                string connectionString = scopedConfiguration["ConnectionString"];
                switch (wellKnownType)
                {
                    case "mongo":
                        logger.LogInformation("PlayerManagement:Store: using 'mongo' store");
                        string databaseName = scopedConfiguration["DatabaseName"];

                        services.AddTransient<IPlayerManagementStore>(serviceProvider =>
                        {
                            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                            // TODO - look at encapsulating the connection info and registering that so that we can just register the type without the factory
                            return new MongoDBPlayerManagementStore(connectionString, databaseName, loggerFactory);
                        });
                        break;
                    case "sql":
                        logger.LogInformation("PlayerManagement:Store: using 'sql' store");
                        services.AddTransient<IPlayerManagementStore>(serviceProvider =>
                        {
                            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                            // TODO - look at encapsulating the connection info and registering that so that we can just register the type without the factory
                            return new SqlPlayerManagementStore(connectionString, loggerFactory);
                        });
                        break;
                    default:
                        throw new Exception($"Unhandled 'wellKnown' type for PlayerManagement:Store: '{wellKnownType}'");
                }
            }
            else
            {
                // fall back to generic "factory"/"implementation" configuration
                services.AddServiceFromConfiguration<IPlayerManagementStore>(configuration, logger, "PlayerManagement:Store");
            }
            return services;
        }
    }
}
