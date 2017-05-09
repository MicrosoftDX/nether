// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nether.Data.Leaderboard
{
    public interface ILeaderboardStore
    {
        Task SaveScoreAsync(GameScore score);
        Task<List<GameScore>> GetAllHighScoresAsync();
        Task<List<GameScore>> GetTopHighScoresAsync(int n);
        Task<List<GameScore>> GetScoresAroundMeAsync(string userId, int radius);
        Task DeleteAllScoresAsync(string userId);
    }
}

