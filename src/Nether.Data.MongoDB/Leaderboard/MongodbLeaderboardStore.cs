// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nether.Data.Leaderboard;
using Microsoft.Extensions.Logging;

namespace Nether.Data.MongoDB.Leaderboard
{
    public class MongoDBLeaderboardStore : ILeaderboardStore
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDBLeaderboardStore> _logger;

        private IMongoCollection<MongoDBGameScore> ScoresCollection
            => _database.GetCollection<MongoDBGameScore>("scores");

        public MongoDBLeaderboardStore(string connectionString, string dbName, ILoggerFactory loggerFactory)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
            _logger = loggerFactory.CreateLogger<MongoDBLeaderboardStore>();
        }

        public async Task SaveScoreAsync(GameScore gameScore)
        {
            _logger.LogDebug("Saving score {0} for gamertag '{1}", gameScore.Score, gameScore.Gamertag);
            await ScoresCollection.InsertOneAsync(gameScore);
        }

        public async Task<List<GameScore>> GetAllHighScoresAsync()
        {
            var query = from s in ScoresCollection.AsQueryable()
                        group s by s.Gamertag
                        into g
                        orderby g.Max(s => s.Score) descending 
                        select new GameScore
                        {
                            Gamertag = g.Key,
                            Score = g.Max(s => s.Score)
                        };

            return await query.ToListAsync();
        }
    }
}

