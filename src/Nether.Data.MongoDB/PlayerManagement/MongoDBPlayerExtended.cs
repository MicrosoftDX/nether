// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics;

namespace Nether.Data.MongoDB.PlayerManagement
{
    [DebuggerDisplay("MongDBPlayerExtended (tag '{Gamertag}', UserId '{Id'})")]
    public class MongoDBPlayerExtended
    {
        // Implicit operator allows Player objects to be used as MongoDbPlayer objects
        public static implicit operator MongoDBPlayerExtended(PlayerState value)
        {
            return new MongoDBPlayerExtended { PlayerId = value.UserId, ExtendedInformation = value.State };
        }

        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string PlayerId { get; internal set; }
        public string Gamertag { get; set; }
        public string ExtendedInformation { get; set; }
    }
}
