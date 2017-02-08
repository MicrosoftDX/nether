// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nether.Data.PlayerManagement;

namespace Nether.Data.MongoDB.PlayerManagement
{
    public class MongoDBPlayer
    {
        // Implicit operator allows Player objects to be used as MongoDbPlayer objects
        public static implicit operator MongoDBPlayer(Player value)
        {
            return new MongoDBPlayer { PlayerId = value.UserId, Gamertag = value.Gamertag, Country = value.Country, CustomTag = value.CustomTag };
        }

        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string PlayerId { get; internal set; }
        public string Gamertag { get; set; }
        public string Country { get; set; }
        public string CustomTag { get; set; }
    }
}
