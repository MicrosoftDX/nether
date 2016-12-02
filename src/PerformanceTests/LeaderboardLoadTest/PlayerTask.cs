// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace LeaderboardLoadTest
{
    public class PlayerTask
    {
        public PlayerTask(AutoPlayer player, Task task)
        {
            Player = player;
            Task = task;
        }

        public AutoPlayer Player { get; }

        public Task Task { get; }
    }
}
