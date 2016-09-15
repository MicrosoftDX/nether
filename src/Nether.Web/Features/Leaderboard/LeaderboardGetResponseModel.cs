using System.Collections.Generic;
using Nether.Data.Leaderboard;

namespace Nether.Web.Features.Leaderboard
{
    public class LeaderboardGetResponseModel
    {
        public List<LeaderboardEntry> LeaderboardEntries { get; set; }

        public class LeaderboardEntry
        {
            public static implicit operator LeaderboardEntry(GameScore score)
            {
                return new LeaderboardEntry {Gamertag = score.Gamertag, Score = score.Score};
            }

            public string Gamertag { get; set; }
            public int Score { get; set; }
        }
    }
}