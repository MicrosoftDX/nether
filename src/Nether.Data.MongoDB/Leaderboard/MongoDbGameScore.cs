using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nether.Leaderboard.Data.Mongodb
{
    public class MongoDbGameScore
    {
        // Implicit operator allows GameScore objects to be used as MongoDbGameScore objects
        public static implicit operator MongoDbGameScore(GameScore value)
        {
            return new MongoDbGameScore { Gamertag = value.Gamertag, Score = value.Score };
        }

        [BsonId]
        public ObjectId TestId { get; set; }
        public string Gamertag { get; set; }
        public int Score { get; set; }
    }
}