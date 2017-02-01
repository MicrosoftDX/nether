// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nether.Data.Sql.Leaderboard
{
    public class SqlLeaderboardStore : ILeaderboardStore
    {
        private LeaderboardContext _db;
        private readonly ILogger<SqlLeaderboardStore> _logger;
        private readonly string _table = "Scores";

        public SqlLeaderboardStore(string connectionString, ILogger<SqlLeaderboardStore> logger)
        {
            _db = new LeaderboardContext(connectionString, _table);
            _logger = logger;
        }

        public async Task SaveScoreAsync(GameScore score)
        {
            await _db.SaveScoreAsync(score);
        }

        public async Task<List<GameScore>> GetAllHighScoresAsync()
        {
            return await _db.GetHighScoresAsync(0);
        }

        public async Task<List<GameScore>> GetTopHighScoresAsync(int n)
        {
            return await _db.GetHighScoresAsync(n);
        }


        public async Task<List<GameScore>> GetScoresAroundMeAsync(string gamerTag, int radius)
        {
            return await _db.GetScoresAroundMeAsync(gamerTag, radius);
        }

        public async Task DeleteAllScoresAsync(string gamerTag)
        {
            await _db.DeleteScores(gamerTag);
        }
    }
}
