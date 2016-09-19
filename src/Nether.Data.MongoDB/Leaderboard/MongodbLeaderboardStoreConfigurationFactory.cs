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
