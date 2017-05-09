// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nether.Data.EntityFramework.Leaderboard
{
    public class EntityFrameworkLeaderboardStore : ILeaderboardStore
    {
        private readonly LeaderboardContextBase _context;
        private readonly ILogger _logger;

        public EntityFrameworkLeaderboardStore(ILogger<EntityFrameworkLeaderboardStore> logger, LeaderboardContextBase context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveScoreAsync(GameScore score)
        {
            await _context.SaveScoreAsync(score);
        }

        public async Task<List<GameScore>> GetAllHighScoresAsync()
        {
            return await _context.GetHighScoresAsync(0);
        }

        public async Task<List<GameScore>> GetTopHighScoresAsync(int n)
        {
            return await _context.GetHighScoresAsync(n);
        }


        public async Task<List<GameScore>> GetScoresAroundMeAsync(string userId, int radius)
        {
            return await _context.GetScoresAroundMeAsync(userId, radius);
        }

        public async Task DeleteAllScoresAsync(string userId)
        {
            await _context.DeleteScoresAsync(userId);
        }
    }
}
