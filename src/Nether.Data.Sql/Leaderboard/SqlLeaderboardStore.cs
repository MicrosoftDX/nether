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
        private ScoreContext _db;
        private readonly ILogger<SqlLeaderboardStore> _logger;
        private readonly string _table = "Scores";

        public SqlLeaderboardStore(string connectionString, ILoggerFactory loggerFactory)
        {
            _db = new ScoreContext(connectionString, _table);
            _logger = loggerFactory.CreateLogger<SqlLeaderboardStore>();
        }

        public async Task SaveScoreAsync(GameScore score)
        {
            await _db.SaveSoreAsync(score);
        }

        public Task<List<GameScore>> GetAllHighScoresAsync()
        {
            return Task.FromResult(_db.GetHighScoresAsync(0));
        }

        public Task<List<GameScore>> GetScoresAroundMe(int nBetter, int nWorse, string gamerTag)
        {
            throw new NotImplementedException();
        }

        public Task<List<GameScore>> GetTopHighScoresAsync(int n)
        {
            return Task.FromResult(_db.GetHighScoresAsync(n));
        }
    }
}
