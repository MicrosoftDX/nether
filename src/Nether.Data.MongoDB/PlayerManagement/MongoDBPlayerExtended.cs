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
    [DebuggerDisplay("MongoDBPlayerExtended (PlayerId '{PlayerId'})")]
    public class MongoDBPlayerExtended
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string PlayerId { get; set; }
        public string ExtendedInformation { get; set; }
    }
}
