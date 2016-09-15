using Microsoft.Extensions.Configuration;
using Nether.Common.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Leaderboard.Data.Mongodb
{
    public class MongodbLeaderboardStoreConfigurationFactory : IDependencyFactory<ILeaderboardStore>
    {
        public ILeaderboardStore CreateInstance(IConfiguration configuration)
        {
            // TODO - explore scoping the configuration to the "properties" section. This would change the code to:
            //                     string connectionString = configuration["ConnectionString"];
            string connectionString = configuration["LeaderboardStore:properties:ConnectionString"];
            string databaseName = configuration["LeaderboardStore:properties:DatabaseName"];
            return new MongodbLeaderboardStore(connectionString, databaseName);
        }
    }
}
