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
            return _db.GetHighScoresAsync();
        }

        public Task<List<GameScore>> GetScoresAroundMe(int nBetter, int nWorse, string gamerTag)
        {
            throw new NotImplementedException();
        }

        public Task<List<GameScore>> GetTopHighScoresAsync(int n)
        {
            throw new NotImplementedException();
        }
        
    }
}
