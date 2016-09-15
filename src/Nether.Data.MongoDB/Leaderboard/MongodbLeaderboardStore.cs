using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nether.Data.Leaderboard;

namespace Nether.Data.MongoDB.Leaderboard
{
    public class MongoDBLeaderboardStore : ILeaderboardStore
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<MongoDBGameScore> ScoresCollection
            => _database.GetCollection<MongoDBGameScore>("scores");

        public MongoDBLeaderboardStore(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
        }

        public async Task SaveScoreAsync(GameScore gameScore)
        {
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
