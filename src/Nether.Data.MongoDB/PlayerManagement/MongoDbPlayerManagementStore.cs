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
using System.Collections;

namespace Nether.Data.MongoDB.PlayerManagement
{
    public class MongoDBPlayerManagementStore : IPlayerManagementStore
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDBPlayerManagementStore> _logger;

        private IMongoCollection<MongoDBPlayer> PlayersCollection
            => _database.GetCollection<MongoDBPlayer>("players");

        private IMongoCollection<MongoDBGroup> GroupsCollection
            => _database.GetCollection<MongoDBGroup>("groups");


        public MongoDBPlayerManagementStore(string connectionString, string dbName, ILoggerFactory loggerFactory)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
            _logger = loggerFactory.CreateLogger<MongoDBPlayerManagementStore>();
        }

        public async Task AddPlayerToGroupAsync(Group group, Player player)
        {
            _logger.LogDebug("Adding players to the group {0}", group.Name);
            group.Players.Add(player);
            await GroupsCollection.InsertOneAsync(group);
        }

        public async Task<Group> GetGroupDetailsAsync(string groupname)
        {
            var getGroup = from s in GroupsCollection.AsQueryable()
                           where s.Name == groupname
                           orderby s.Name descending
                           select new Group
                           {
                               Name = s.Name,
                               CustomType = s.CustomType,
                               Description = s.Description,
                               Image = s.Image,
                               Players = s.Players
                           };

            return await getGroup.FirstOrDefaultAsync();
        }

        public async Task<Player> GetPlayerDetailsAsync(string gamertag)
        {
            var getPlayer = from s in PlayersCollection.AsQueryable()
                            where s.Gamertag == gamertag
                            orderby s.Gamertag descending
                            select new Player
                            {
                                PlayerId = s.PlayerId,
                                Gamertag = s.Gamertag,
                                Country = s.Country,
                                CustomTag = s.CustomTag,
                                PlayerImage = s.PlayerImage
                            };

            return await getPlayer.FirstOrDefaultAsync();
        }
        public async Task<Player> GetPlayerDetailsByIdAsync(string id)
        {
            var getPlayer = from s in PlayersCollection.AsQueryable()
                            where s.PlayerId == id
                            orderby s.PlayerId descending
                            select new Player
                            {
                                PlayerId = s.PlayerId,
                                Gamertag = s.Gamertag,
                                Country = s.Country,
                                CustomTag = s.CustomTag,
                                PlayerImage = s.PlayerImage
                            };

            return await getPlayer.FirstOrDefaultAsync();
        }


        public async Task RemovePlayerFromGroupAsync(Group group, Player player)
        {
            //Not implemented for now. The SaveGroupAsync method can be used to send an updated players list instead.
            await SaveGroupAsync(group);
        }

        public async Task SaveGroupAsync(Group group)
        {
            _logger.LogDebug("Saving Group {0}", group.Name);
            await GroupsCollection.InsertOneAsync(group);
        }

        public async Task SavePlayerAsync(Player player)
        {
            _logger.LogDebug("Saving Player {0}", player.Gamertag);
            await PlayersCollection.InsertOneAsync(player);
        }

        public async Task<List<Player>> GetGroupPlayersAsync(string groupname)
        {
            var query = from s in GroupsCollection.AsQueryable()
                        where s.Name == groupname
                        orderby s.Name descending
                        select new Group
                        {
                            Name = s.Name,
                            CustomType = s.CustomType,
                            Description = s.Description,
                            Image = s.Image,
                            Players = s.Players
                        };

            var groupplayers = await query.FirstOrDefaultAsync();

            return groupplayers.Players;
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            var getPlayer = from s in PlayersCollection.AsQueryable()
                            orderby s.Gamertag descending
                            select new Player
                            {
                                PlayerId = s.PlayerId,
                                Gamertag = s.Gamertag,
                                Country = s.Country,
                                CustomTag = s.CustomTag,
                                PlayerImage = s.PlayerImage
                            };

            return await getPlayer.ToListAsync();
        }

        public async Task<List<Group>> GetPlayersGroupsAsync(string gamertag)
        {
            var result = new List<Group>();

            var getGroup = from s in GroupsCollection.AsQueryable()
                           orderby s.Name descending
                           select new Group
                           {
                               Name = s.Name,
                               CustomType = s.CustomType,
                               Description = s.Description,
                               Image = s.Image,
                               Players = s.Players
                           };
            await getGroup.ForEachAsync(g =>
            {
                foreach (Player p in g.Players)
                {
                    if (p.Gamertag == gamertag)
                    {
                        result.Add(g);
                    }
                }
            });
            return result;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            var getGroup = from s in GroupsCollection.AsQueryable()
                           orderby s.Name descending
                           select new Group
                           {
                               Name = s.Name,
                               CustomType = s.CustomType,
                               Description = s.Description,
                               Image = s.Image,
                               Players = s.Players
                           };

            return await getGroup.ToListAsync();
        }

        public async Task UploadPlayerImageAsync(string gamertag, byte[] image)
        {
            _logger.LogDebug("Saving Player image {0}", gamertag);

            var filter = Builders<MongoDBPlayer>.Filter.Eq(s => s.Gamertag, gamertag);
            var update = Builders<MongoDBPlayer>.Update.Set(s => s.PlayerImage, image);
            await PlayersCollection.UpdateOneAsync(filter, update);
        }

        public async Task<byte[]> GetPlayerImageAsync(string gamertag)
        {
            var getPlayer = from s in PlayersCollection.AsQueryable()
                            where s.Gamertag == gamertag
                            orderby s.Gamertag descending
                            select new Player
                            {
                                Gamertag = s.Gamertag,
                                Country = s.Country,
                                CustomTag = s.CustomTag,
                                PlayerImage = s.PlayerImage
                            };

            Player p = await getPlayer.FirstOrDefaultAsync();

            return p.PlayerImage;
        }

        public async Task UploadGroupImageAsync(string groupname, byte[] image)
        {
            _logger.LogDebug("Saving Group image {0}", groupname);

            var filter = Builders<MongoDBGroup>.Filter.Eq(s => s.Name, groupname);
            var update = Builders<MongoDBGroup>.Update.Set(s => s.Image, image);
            await GroupsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<byte[]> GetGroupImageAsync(string name)
        {
            var getGroup = from s in GroupsCollection.AsQueryable()
                           where s.Name == name
                           orderby s.Name descending
                           select new Group
                           {
                               Name = s.Name,
                               CustomType = s.CustomType,
                               Description = s.Description,
                               Image = s.Image,
                               Players = s.Players
                           };

            Group g = await getGroup.FirstOrDefaultAsync();

            return g.Image;
        }
    }
}
