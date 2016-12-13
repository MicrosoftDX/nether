// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        private FactContext _factDb;
        private readonly string _factTable = "PlayerManagementFact";

        private readonly ILogger<SqlPlayerManagementStore> _logger;

        public SqlPlayerManagementStore(String connectionString, ILoggerFactory loggerFactory)
        {
            _playerDb = new PlayerContext(connectionString, _playerTable);
            _groupDb = new GroupContext(connectionString, _groupTable);
            _factDb = new FactContext(connectionString, _factTable);
            _logger = loggerFactory.CreateLogger<SqlPlayerManagementStore>();
        }

        public async Task AddPlayerToGroupAsync(Group group, Player player)
        {
            // assuming that thhe player and the group already exist 
            await _factDb.AddPlayerToGroupAsync(group, player.PlayerId);
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
            List<string> groupPlayers = await _factDb.getGroupPlayersAsync(groupname);
            return groupPlayers;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _groupDb.GetGroupsAsync();
        }

        public async Task<Player> GetPlayerDetailsAsync(string gamertag)
        {
            return await _playerDb.GetPlayerDetailsAsync(gamertag);
        }

        public async Task<Player> GetPlayerDetailsByIdAsync(string id)
        {
            return await _playerDb.GetPlayerDetailsByIdAsync(id);
        }

        public async Task<byte[]> GetPlayerImageAsync(string gamertag)
        {
            return await _playerDb.GetPlayerImageAsync(gamertag);
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            return await _playerDb.GetPlayersAsync();
        }

        public async Task<List<Group>> GetPlayersGroupsAsync(string gamertag)
        {
            // get the playerid for gamertag
            string playerId = await _playerDb.GetPlayerIdForGamerTag(gamertag);
            // get all the groups for player 
            List<string> playerGroups = await _factDb.GetPlayerGroupsAsync(playerId);
            return playerGroups.Select(g => GetGroupDetailsAsync(g).Result).ToList();
        }

        public async Task RemovePlayerFromGroupAsync(Group group, Player player)
        {
            await _factDb.RemovePlayerFromGroupAsync(group.Name, player.PlayerId);
        }

        public async Task SaveGroupAsync(Group group)
        {
            // add a new group if does not exist
            await _groupDb.SaveGroupAsync(group);

            // add a new player if does not exist and update the fact table with the relation between player and group
            if (group.Members != null)
            {
                foreach (string playerGamerTag in group.Members)
                {
                    await _factDb.AddPlayerToGroupAsync(group, playerGamerTag);
                }
            }
        }

        public async Task SavePlayerAsync(Player player)
        {
            await _playerDb.SavePlayerAsync(player);
        }

        public async Task UploadGroupImageAsync(string groupname, byte[] image)
        {
            await _groupDb.UploadGroupImageAsync(groupname, image);
        }

        public async Task UploadPlayerImageAsync(string gamertag, byte[] image)
        {
            await _playerDb.UploadPlayerImageAsync(gamertag, image);
        }
    }
}
