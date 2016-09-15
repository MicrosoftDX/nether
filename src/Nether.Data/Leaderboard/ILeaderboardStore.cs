using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nether.Data.Leaderboard
{
    public interface ILeaderboardStore
    {
        Task SaveScoreAsync(GameScore score);
        Task<List<GameScore>> GetAllHighScoresAsync();
    }
}
