// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.PlayerManagement
{
    public interface IPlayerManagementStore
    {
        //Players
        Task SavePlayerAsync(Player player);
        Task<Player> GetPlayerDetailsByUserIdAsync(string id);
        Task<Player> GetPlayerDetailsAsync(string gamertag);
        Task<List<Player>> GetPlayersAsync();
        Task<List<Group>> GetPlayersGroupsAsync(string gamertag);
        Task UploadPlayerImageAsync(string gamertag, byte[] image);
        Task<byte[]> GetPlayerImageAsync(string gamertag);

        //Group
        Task SaveGroupAsync(Group group);
        Task<Group> GetGroupDetailsAsync(string groupname);
        Task AddPlayerToGroupAsync(Group group, Player player);
        Task RemovePlayerFromGroupAsync(Group group, Player player);
        Task<List<string>> GetGroupPlayersAsync(string groupname);
        Task<List<Group>> GetGroupsAsync();
        Task UploadGroupImageAsync(string groupname, byte[] image);
        Task<byte[]> GetGroupImageAsync(string name);
    }
}
