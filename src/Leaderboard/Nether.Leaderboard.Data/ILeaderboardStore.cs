using System.Threading.Tasks;

namespace Nether.Leaderboard.Data
{
    public interface ILeaderboardStore
    {
        Task SaveScoreAsync(string playerId, int score);
        Task<int> GetScoreAsync(string v);
    }
}
