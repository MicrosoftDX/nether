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
                                Gamertag = s.Gamertag,
                                Country = s.Country,
                                CustomTag = s.CustomTag,
                                PlayerImage = s.PlayerImage
                            };

            return await getPlayer.FirstOrDefaultAsync();
        }

        public Task RemovePlayerFromGroupAsync(Group group, Player player)
        {
            throw new NotImplementedException();
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
    }
}
