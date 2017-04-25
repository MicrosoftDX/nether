// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nether.Data.PlayerManagement;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Nether.Data.MongoDB.PlayerManagement
{
    public class MongoDBPlayerManagementStore : IPlayerManagementStore
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger _logger;

        private IMongoCollection<MongoDBPlayer> PlayersCollection
            => _database.GetCollection<MongoDBPlayer>("players");

        private IMongoCollection<MongoDBPlayerExtended> PlayersExtendedCollection
            => _database.GetCollection<MongoDBPlayerExtended>("playersextended");

        private static readonly UpdateOptions s_upsertOptions = new UpdateOptions { IsUpsert = true };

        public MongoDBPlayerManagementStore(string connectionString, string dbName, ILogger<MongoDBPlayerManagementStore> logger)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
            _logger = logger;

            // ensure PlayerId is indexed as we query by this
            PlayersCollection.Indexes.CreateOne(Builders<MongoDBPlayer>.IndexKeys.Ascending(_ => _.PlayerId));
        }

        public async Task<Player> GetPlayerDetailsByGamertagAsync(string gamertag)
        {
            var getPlayer = from s in PlayersCollection.AsQueryable()
                            where s.Gamertag == gamertag
                            orderby s.Gamertag descending
                            select new Player
                            {
                                UserId = s.PlayerId,
                                Gamertag = s.Gamertag,
                                Country = s.Country,
                                CustomTag = s.CustomTag
                            };

            return await getPlayer.FirstOrDefaultAsync();
        }
        public async Task<Player> GetPlayerDetailsByUserIdAsync(string id)
        {
            var getPlayer = from s in PlayersCollection.AsQueryable()
                            where s.PlayerId == id
                            orderby s.PlayerId descending
                            select new Player
                            {
                                UserId = s.PlayerId,
                                Gamertag = s.Gamertag,
                                Country = s.Country,
                                CustomTag = s.CustomTag
                            };

            return await getPlayer.FirstOrDefaultAsync();
        }

        public async Task SavePlayerAsync(Player player)
        {
            _logger.LogDebug("Saving Player {0}", player.Gamertag);
            await PlayersCollection.ReplaceOneAsync(p => p.PlayerId == player.UserId, player, s_upsertOptions);
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            var getPlayer = from s in PlayersCollection.AsQueryable()
                            orderby s.Gamertag descending
                            select new Player
                            {
                                UserId = s.PlayerId,
                                Gamertag = s.Gamertag,
                                Country = s.Country,
                                CustomTag = s.CustomTag
                            };

            return await getPlayer.ToListAsync();
        }

        public async Task SavePlayerStateByUserIdAsync(string userId, string state)
        {
            _logger.LogDebug("Saving Player Extended {0}", userId);
            await PlayersExtendedCollection.ReplaceOneAsync(
                p => p.PlayerId == userId,
                new MongoDBPlayerExtended { PlayerId = userId, ExtendedInformation = state },
                s_upsertOptions);
        }

        public async Task<string> GetPlayerStateByUserIdAsync(string userId)
        {
            var getPlayerExtended = from s in PlayersExtendedCollection.AsQueryable()
                                    where s.PlayerId == userId
                                    orderby s.PlayerId descending
                                    select s.ExtendedInformation;

            var playerState = await getPlayerExtended.FirstOrDefaultAsync();
            return playerState;
        }

        public Task DeletePlayerDetailsForUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
