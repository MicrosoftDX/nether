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
            await _playerGroupDb.AddPlayerToGroupAsync(group, player.Gamertag);
        }

        public async Task<Group> GetGroupDetailsAsync(string groupname)
        {
            return await _groupDb.GetGroupDetailsAsync(groupname);
        }

        public async Task<byte[]> GetGroupImageAsync(string name)
        {
            return await _groupDb.GetGroupImageAsync(name);
        }

        public async Task<List<string>> GetGroupPlayersAsync(string groupname)
        {
            // get all the players for groupname
            List<string> groupPlayers = await _playerGroupDb.GetGroupPlayersAsync(groupname);
            return groupPlayers;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _groupDb.GetGroupsAsync();
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
            List<string> playerGroups = await _playerGroupDb.GetPlayerGroupsAsync(gamerTag);

            return playerGroups.Select(g => GetGroupDetailsAsync(g).Result).ToList();
        }

        public async Task RemovePlayerFromGroupAsync(Group group, Player player)
        {
            await _playerGroupDb.RemovePlayerFromGroupAsync(group.Name, player.PlayerId);
        }

        public async Task SaveGroupAsync(Group group)
        {
            // add a new group if does not exist
            await _groupDb.SaveGroupAsync(group);

            // add a new player if does not exist and update the player<->group mapping table with the relation between player and group
            if (group.Members != null)
            {
                foreach (string playerGamerTag in group.Members)
                {
                    await _playerGroupDb.AddPlayerToGroupAsync(group, playerGamerTag);
                }
            }
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
            await _groupDb.UploadGroupImageAsync(groupname, image);
        }

        public Task UploadPlayerImageAsync(string gamertag, byte[] image)
        {
            throw new NotSupportedException();
        }
    }
}
