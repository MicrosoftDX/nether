// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using System;

namespace Nether.Data.MongoDB.Leaderboard
{
    public class MongoDBLeaderboardStoreConfigurationFactory : IDependencyFactory<ILeaderboardStore>
    {
        public ILeaderboardStore CreateInstance(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>().GetSection("LeaderboardStore:properties");
            string connectionString = configuration["ConnectionString"];
            string databaseName = configuration["DatabaseName"];

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            return new MongoDBLeaderboardStore(connectionString, databaseName, loggerFactory);
        }
    }
}

