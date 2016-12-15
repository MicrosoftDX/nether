// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.PlayerManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.PlayerManagement
{
    public class SqlPlayerManagementStore : IPlayerManagementStore
    {
        private PlayerContext _playerDb;
        private readonly string _playerTable = "Players";
        private GroupContext _groupDb;
        private readonly string _groupTable = "Groups";
        private PlayerGroupContext _playerGroupDb;
        private readonly string _playerGroupTable = "PlayerGroups";

        private readonly ILogger<SqlPlayerManagementStore> _logger;

        public SqlPlayerManagementStore(string connectionString, ILoggerFactory loggerFactory)
        {
            _playerDb = new PlayerContext(connectionString, _playerTable);
            _groupDb = new GroupContext(connectionString, _groupTable);
            _playerGroupDb = new PlayerGroupContext(connectionString, _playerGroupTable, loggerFactory);
            _logger = loggerFactory.CreateLogger<SqlPlayerManagementStore>();
        }

        public async Task AddPlayerToGroupAsync(Group group, Player player)
        {
            // assuming that thhe player and the group already exist 
            await AddPlayerToGroupAsync(group.Name, player.Gamertag);
        }

        private async Task AddPlayerToGroupAsync(string groupName, string gamerTag)
        {
            PlayerEntity dbPlayer = await _playerGroupDb.Players
                                                .Where(p => p.Gamertag == gamerTag)
                                                .FirstOrDefaultAsync();

            if (dbPlayer == null)
                throw new ArgumentException($"player '{gamerTag}' does not exist", nameof(gamerTag));

            GroupEntity dbGroup = await _playerGroupDb.Groups
                                            .Where(g => g.Name == groupName)
                                            .FirstOrDefaultAsync();
            if (dbGroup == null)
                throw new ArgumentException($"group '{groupName}' does not exist", nameof(groupName));

            await _playerGroupDb.PlayerGroups.AddAsync(new PlayerGroupEntity
            {
                Player = dbPlayer,
                Group = dbGroup
            });
            await _playerGroupDb.SaveChangesAsync();
        }

        public async Task<Group> GetGroupDetailsAsync(string groupname)
        {
            var group = await _groupDb.Groups.SingleAsync(g => g.Name.Equals(groupname));
            return new Group
            {
                Name = group.Name,
                CustomType = group.CustomType,
                Description = group.Description
            };
        }

        public async Task<byte[]> GetGroupImageAsync(string name)
        {
            var group = await _groupDb.Groups.SingleAsync(g => g.Name.Equals(name));
            return group.Image;
        }

        public async Task<List<string>> GetGroupPlayersAsync(string groupName)
        {
            // get all the players for groupname
            List<string> groupPlayersGamertags = await _playerGroupDb.PlayerGroups
                .Where(map => map.Group.Name == groupName)
                .Select(map => map.Player.Gamertag)
                .AsNoTracking()
                .ToListAsync();

            return groupPlayersGamertags;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _groupDb.Groups.Select(g => new Group
            {
                Name = g.Name,
                CustomType = g.CustomType,
                Description = g.Description
            }).ToListAsync();
        }

        public async Task<Player> GetPlayerDetailsAsync(string gamertag)
        {
            PlayerEntity player = await _playerDb.Players.SingleOrDefaultAsync(p => p.Gamertag.Equals(gamertag));
            return player?.ToPlayer();
        }

        public async Task<Player> GetPlayerDetailsByIdAsync(string id)
        {
            PlayerEntity player = await _playerDb.Players.SingleOrDefaultAsync(p => p.PlayerId.Equals(id));
            return player?.ToPlayer();
        }

        public Task<byte[]> GetPlayerImageAsync(string gamertag)
        {
            throw new NotSupportedException();
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            return await _playerDb.Players.Select(p => p.ToPlayer()).ToListAsync();
        }

        public async Task<List<Group>> GetPlayersGroupsAsync(string gamerTag)
        {
            if (gamerTag == null) throw new ArgumentNullException(nameof(gamerTag));

            // get all the group names for player 
            List<string> playerGroups = await GetPlayerGroupsAsync(gamerTag);

            return playerGroups.Select(g => GetGroupDetailsAsync(g).Result).ToList();
        }
        private async Task<List<string>> GetPlayerGroupsAsync(string gamerTag)
        {
            List<string> groupNames = await _playerGroupDb.PlayerGroups
                .Where(map => map.Player.Gamertag == gamerTag)
                .Select(map => map.Group.Name)
                .ToListAsync();

            return groupNames;
        }

        public async Task RemovePlayerFromGroupAsync(Group group, Player player)
        {

            var playerGroups = await _playerGroupDb.PlayerGroups
                                                .Where(map => map.Group.Name == group.Name && map.Player.PlayerId == player.PlayerId)
                                                .ToListAsync();

            _playerGroupDb.RemoveRange(playerGroups);
            await _playerGroupDb.SaveChangesAsync();
        }

        public async Task SaveGroupAsync(Group group)
        {
            // add a new group if does not exist
            await SaveGroupEntityAsync(group);

            // add a new player if does not exist and update the player<->group mapping table with the relation between player and group
            if (group.Members != null)
            {
                foreach (string playerGamerTag in group.Members)
                {
                    await AddPlayerToGroupAsync(group.Name, playerGamerTag);
                }
            }
        }
        public async Task<Guid> SaveGroupEntityAsync(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            // add new group only if it does not exist
            GroupEntity entity = await _groupDb.Groups.FindAsync(group.Name);
            if (entity == null)
            {
                var newGroup = new GroupEntity
                {
                    Name = group.Name,
                    CustomType = group.CustomType,
                    Description = group.Description
                };

                await _groupDb.Groups.AddAsync(newGroup);
                await _groupDb.SaveChangesAsync();
                entity = newGroup;
            }
            else
            {
                entity.CustomType = group.CustomType;
                entity.Description = group.Description;
                await _groupDb.SaveChangesAsync();
            }

            return entity.Id;
        }

        public async Task SavePlayerAsync(Player player)
        {
            // add only of the player does not exist
            PlayerEntity entity = player.PlayerId == null ? null : await _playerDb.Players.FindAsync(player.PlayerId);
            if (entity == null)
            {
                await _playerDb.Players.AddAsync(new PlayerEntity
                {
                    PlayerId = player.PlayerId,
                    Gamertag = player.Gamertag,
                    Country = player.Country,
                    CustomTag = player.CustomTag,
                });
                await _playerDb.SaveChangesAsync();
            }
            else
            {
                entity.Gamertag = player.Gamertag;
                entity.Country = player.Country;
                entity.CustomTag = player.CustomTag;
                await _playerDb.SaveChangesAsync();
            }
        }

        public async Task UploadGroupImageAsync(string groupname, byte[] image)
        {
            var group = await _groupDb.Groups.SingleAsync(g => g.Name.Equals(groupname));
            group.Image = image;
            _groupDb.Groups.Update(group);
            await _groupDb.SaveChangesAsync();
        }

        public Task UploadPlayerImageAsync(string gamertag, byte[] image)
        {
            throw new NotSupportedException();
        }
    }
}
