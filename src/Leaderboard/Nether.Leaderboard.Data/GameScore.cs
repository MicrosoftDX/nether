using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Leaderboard.Data
{
    public class GameScore
    {
        public GameScore()
        {

        }
        public GameScore(string gamertag, int score)
        {
            Gamertag = gamertag;
            Score = score;
        }

        public string Gamertag { get; set; }
        public int Score { get; set; }
    }
}
