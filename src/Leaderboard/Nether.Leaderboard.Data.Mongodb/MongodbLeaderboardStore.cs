using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Nether.Leaderboard.Data.Mongodb
{
    public class MongodbLeaderboardStore : ILeaderboardStore
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        private static string _collectionName = "Leaderboard";

        public MongodbLeaderboardStore(string connectionString) // connect to local "test" db
        {
            _client = new MongoClient();
            _database = _client.GetDatabase(connectionString);
        }

        public Task<IEnumerable<GameScore>> GetScoresAsync()
        {
            return null;
        }

        public async Task SaveScoreAsync(string gamertag, int score)
        {
            var document = new BsonDocument
            {
                {gamertag, score}
            };

            var collection = _database.GetCollection<BsonDocument>(_collectionName);
            await collection.InsertOneAsync(document);
        }
    }
}
