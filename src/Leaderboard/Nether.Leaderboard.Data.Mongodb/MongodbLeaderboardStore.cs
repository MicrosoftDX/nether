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

        public MongodbLeaderboardStore(string connectionString) 
        {
            _client = new MongoClient();
            _database = _client.GetDatabase(connectionString);
        }

        public async Task<IEnumerable<GameScore>> GetScoresAsync()
        {
            List<GameScore> result = new List<GameScore>();
            try
            {                
                var collection = _database.GetCollection<BsonDocument>(_collectionName);

                var aggregate = collection.Aggregate().Group(new BsonDocument { { "_id", "$gamertag" }, { "highScore", new BsonDocument("$max", "$score") } });                
                foreach (var document in aggregate.ToList())
                {
                    result.Add(new GameScore(document.GetValue("_id").AsString, document.GetValue("highScore").AsInt32));
                }
               

                return result;
            }
            catch (Exception e)
            {
                return result;               
            }
        }

        
        public async Task SaveScoreAsync(string gamertag, int score)
        {
            var document = new BsonDocument
            {
                {"gamertag", gamertag },
                { "score" ,score}
            };

            var collection = _database.GetCollection<BsonDocument>(_collectionName);
            try
            {
                await collection.InsertOneAsync(document);
            }
            catch (Exception e)
            {

            }
        }
        
    }
}
