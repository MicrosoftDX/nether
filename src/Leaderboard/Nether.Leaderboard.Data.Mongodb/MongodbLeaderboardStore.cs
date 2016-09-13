using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Nether.Leaderboard.Data.Mongodb
{
    public class MongodbLeaderboardStore : ILeaderboardStore
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<MongoDbGameScore> ScoresCollection
            => _database.GetCollection<MongoDbGameScore>("scores");

        public MongodbLeaderboardStore(string connectionString, string dbName)
        {
            //TODO: Implement full support for configurable server and database
            var client = new MongoClient();
            _database = client.GetDatabase("nether-leaderboard");
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
