using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Leaderboard.Data;

namespace Nether.Leaderboard.Data.InMemory
{
    public class InMemoryLeaderboardStore : ILeaderboardStore
    {
        private static int _score = 0;

        public Task<int> GetScoreAsync(string v)
        {
            return Task.FromResult(_score);
        }

        public Task SaveScoreAsync(string playerId, int score)
        {
            _score = score;
            return Task.CompletedTask;
        }
    }
}
