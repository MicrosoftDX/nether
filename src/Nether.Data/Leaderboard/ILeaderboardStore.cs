using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nether.Leaderboard.Data
{
    public interface ILeaderboardStore
    {
        Task SaveScoreAsync(GameScore score);
        Task<List<GameScore>> GetAllHighScoresAsync();
    }
}
