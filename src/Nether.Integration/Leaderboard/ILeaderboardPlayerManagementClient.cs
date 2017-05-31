// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nether.Integration.Leaderboard
{
    /// <summary>
    /// Client used to allow the Leaderboard feature to integrate with Player Management
    /// in a pluggable manner
    /// </summary>
    public interface ILeaderboardPlayerManagementClient
    {
        Task<UserIdGamertagMap[]> GetGamertagsForUserIdsAsync(string[] userIds);
    }
    public class UserIdGamertagMap
    {
        public string UserId { get; set; }
        public string Gamertag { get; set; }
    }
}
