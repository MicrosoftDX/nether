using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;
using System;

namespace Nether.Data.MongoDB.Leaderboard
{
    public class MongoDBLeaderboardStoreConfigurationFactory : IDependencyFactory<ILeaderboardStore>
    {
        public ILeaderboardStore CreateInstance(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService<IConfiguration>();

            // TODO - explore scoping the configuration to the "properties" section. This would change the code to:
            //                     string connectionString = configuration["ConnectionString"];
            var connectionString = configuration["LeaderboardStore:properties:ConnectionString"];
            var databaseName = configuration["LeaderboardStore:properties:DatabaseName"];
            return new MongoDBLeaderboardStore(connectionString, databaseName);
        }
    }
}
