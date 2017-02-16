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

        private IMongoCollection<MongoDBGroup> GroupsCollection
            => _database.GetCollection<MongoDBGroup>("groups");

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

            // ensure group name is indexed as we query by this too
            GroupsCollection.Indexes.CreateOne(Builders<MongoDBGroup>.IndexKeys.Ascending(_ => _.Name));
        }

        public async Task AddPlayerToGroupAsync(Group group, Player player)
        {
            _logger.LogDebug("Adding players to the group {0}", group.Name);
            if (group.Members == null) group.Members = new List<string>();
            group.Members.Add(player.Gamertag);

            await SaveGroupAsync(group);
        }

        public async Task<Group> GetGroupDetailsAsync(string groupName)
        {
            var query = from s in GroupsCollection.AsQueryable()
                        where s.Name == groupName
                        orderby s.Name descending
                        select s;

            var group = await query.FirstOrDefaultAsync();

            return group?.ToGroup();
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


        public async Task RemovePlayerFromGroupAsync(Group group, Player player)
        {
            if (group.Members == null) return;
            int removedCount = group.Members.RemoveAll(tag => tag == player.Gamertag);

            if (removedCount > 0)
            {
                await SaveGroupAsync(group);
            }
        }

        public async Task SaveGroupAsync(Group group)
        {
            _logger.LogDebug("Saving Group {0}", group.Name);
            await GroupsCollection.ReplaceOneAsync(g => g.Name == group.Name, group, s_upsertOptions);
        }

        public async Task SavePlayerAsync(Player player)
        {
            _logger.LogDebug("Saving Player {0}", player.Gamertag);
            await PlayersCollection.ReplaceOneAsync(p => p.PlayerId == player.UserId, player, s_upsertOptions);
        }

        public async Task<List<string>> GetGroupPlayersAsync(string groupName)
        {
            Group g = await GetGroupDetailsAsync(groupName);
            return g?.Members;
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

        public async Task<List<Group>> GetPlayersGroupsAsync(string gamerTag)
        {
            var query = from g in GroupsCollection.AsQueryable()
                        where g.Members.Any(m => m == gamerTag)
                        select g;
            var groups = await query.ToListAsync();

            _logger.LogDebug("found {0} groups", groups.Count);

            return groups.Select(g => g.ToGroup()).ToList();
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            var query = from s in GroupsCollection.AsQueryable()
                        orderby s.Name descending
                        select s;

            var mGroups = await query.ToListAsync();
            return mGroups.Select(m => m.ToGroup()).ToList();
        }

        public Task UploadPlayerImageAsync(string gamertag, byte[] image)
        {
            throw new NotSupportedException();
        }

        public Task<byte[]> GetPlayerImageAsync(string gamertag)
        {
            throw new NotSupportedException();
        }

        public Task UploadGroupImageAsync(string groupname, byte[] image)
        {
            throw new NotImplementedException();

            /*_logger.LogDebug("Saving Group image {0}", groupname);

            var filter = Builders<MongoDBGroup>.Filter.Eq(s => s.Name, groupname);
            var update = Builders<MongoDBGroup>.Update.Set(s => s.Image, image);
            await GroupsCollection.UpdateOneAsync(filter, update);*/
        }

        public Task<byte[]> GetGroupImageAsync(string name)
        {
            throw new NotImplementedException();

            /*var getGroup = from s in GroupsCollection.AsQueryable()
                           where s.Name == name
                           orderby s.Name descending
                           select new Group
                           {
                               Name = s.Name,
                               CustomType = s.CustomType,
                               Description = s.Description,
                               Members = s.Members
                           };

            Group g = await getGroup.FirstOrDefaultAsync();

            return g.Image;*/
        }

        public async Task SavePlayerExtendedAsync(PlayerState player)
        {
            _logger.LogDebug("Saving Player Extended {0}", player.UserId);
            await PlayersExtendedCollection.ReplaceOneAsync(p => p.PlayerId == player.UserId, player, s_upsertOptions);
        }

        public async Task<PlayerState> GetPlayerDetailsExtendedAsync(string userid)
        {
            var getPlayerExtended = from s in PlayersExtendedCollection.AsQueryable()
                                    where s.PlayerId == userid
                                    orderby s.Gamertag descending
                                    select new PlayerState
                                    {
                                        UserId = s.PlayerId,
                                        Gamertag = s.Gamertag,
                                        State = s.ExtendedInformation
                                    };

            return await getPlayerExtended.FirstOrDefaultAsync();
        }

        public Task DeletePlayerDetailsAsync(string gamertag)
        {
            throw new NotImplementedException();
        }
    }
}
