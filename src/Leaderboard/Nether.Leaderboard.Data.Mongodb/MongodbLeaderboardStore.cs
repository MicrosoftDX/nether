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
        private static string _gamertagLabel = "gamertag";
        private static string _scoreLabel = "score";

        public MongodbLeaderboardStore(string connectionString, string dbName) 
        {            
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(dbName);
        }

        public async Task<IEnumerable<GameScore>> GetScoresAsync()
        {                                   
            var collection = _database.GetCollection<BsonDocument>(_collectionName);           
            var all = collection.Find(new BsonDocument()).ToList();
           
            var query = (from l in all
                         group l by l.GetValue(_gamertagLabel) into lbgt
                         let topscore = lbgt.Max(x => x.GetValue(_scoreLabel))
                         select new GameScore
                         {
                             Gamertag = lbgt.Key.AsString,
                             Score = topscore.AsInt32
                         });

            return query.ToList();
        }

        
        public async Task SaveScoreAsync(string gamertag, int score)
        {
            var document = new BsonDocument
            {
                {_gamertagLabel, gamertag },
                {_scoreLabel ,score}
            };

            var collection = _database.GetCollection<BsonDocument>(_collectionName);
            await collection.InsertOneAsync(document);            
        }
        
    }
}
