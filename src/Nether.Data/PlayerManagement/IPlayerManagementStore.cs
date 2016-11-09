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
        Task SavePlayerAsync(Player player);
        Task<Player> GetPlayerDetailsAsync(string gamertag);

        Task SaveGroupAsync(Group group);
        Task<Group> GetGroupDetailsAsync(string groupname);
        Task AddPlayerToGroupAsync(Group group, Player player);
        Task RemovePlayerFromGroupAsync(Group group, Player player);
    }
}
