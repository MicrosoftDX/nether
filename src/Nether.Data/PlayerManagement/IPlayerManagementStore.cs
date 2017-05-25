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
        // Players
        Task SavePlayerAsync(Player player);
        Task<Player> GetPlayerDetailsByUserIdAsync(string id);
        Task<Player[]> GetPlayerDetailsByUserIdsAsync(string[] ids);
        Task<Player> GetPlayerDetailsByGamertagAsync(string gamertag);
        Task<List<Player>> GetPlayersAsync();
        Task<string> GetPlayerStateByUserIdAsync(string id);
        Task SavePlayerStateByUserIdAsync(string id, string state);
        Task DeletePlayerDetailsForUserIdAsync(string id);
    }
}
