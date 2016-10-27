// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nether.Data.Leaderboard;

namespace Nether.Data.MongoDB.Leaderboard
{
    public class MongoDBGameScore
    {
        // Implicit operator allows GameScore objects to be used as MongoDbGameScore objects
        public static implicit operator MongoDBGameScore(GameScore value)
        {
            return new MongoDBGameScore { Gamertag = value.Gamertag, Country = value.Country, CustomTag = value.CustomTag, Score = value.Score };
        }

        [BsonId]
        public ObjectId TestId { get; set; }
        public string Gamertag { get; set; }
        public string Country { get; set; }
        public string CustomTag { get; set; }
        public int Score { get; set; }
    }
}
