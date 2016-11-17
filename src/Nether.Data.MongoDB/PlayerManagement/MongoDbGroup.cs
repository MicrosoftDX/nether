// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nether.Data.PlayerManagement;

namespace Nether.Data.MongoDB.PlayerManagement
{
    public class MongoDBGroup
    {
        // Implicit operator allows Group objects to be used as MongoDBGroup objects
        public static implicit operator MongoDBGroup(Group value)
        {
            return new MongoDBGroup
            {
                Name = value.Name,
                CustomType = value.CustomType,
                Description = value.Description,
                Image = value.Image,
                Players = value.Players
            };
        }

        [BsonId]
        public ObjectId TestId { get; set; }
        public string Name { get; set; }
        public string CustomType { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public List<Player> Players { get; set; }
    }
}
