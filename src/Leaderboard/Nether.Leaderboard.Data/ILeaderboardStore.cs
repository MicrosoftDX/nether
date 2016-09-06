using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nether.Leaderboard.Data
{
    public interface ILeaderboardStore
    {
        Task SaveScoreAsync(string gamertag, int score);
        Task<Dictionary<string, int>> GetScoresAsync();
    }
}
