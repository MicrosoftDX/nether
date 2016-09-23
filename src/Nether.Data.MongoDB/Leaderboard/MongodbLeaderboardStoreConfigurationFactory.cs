using Microsoft.Extensions.Configuration;
using Nether.Common.DependencyInjection;
using Nether.Data.Leaderboard;

namespace Nether.Data.MongoDB.Leaderboard
{
    public class MongoDBLeaderboardStoreConfigurationFactory : IDependencyFactory<ILeaderboardStore>
    {
        public ILeaderboardStore CreateInstance(IConfiguration configuration)
        {
            // TODO - explore scoping the configuration to the "properties" section. This would change the code to:
            //                     string connectionString = configuration["ConnectionString"];
            var connectionString = configuration["LeaderboardStore:properties:ConnectionString"];
            var databaseName = configuration["LeaderboardStore:properties:DatabaseName"];
            return new MongoDBLeaderboardStore(connectionString, databaseName);
        }
    }
}
