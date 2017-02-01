using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.Leaderboard
{
    public class QueriedGamerScore
    {
        public int Score { get; set; }
        public string GamerTag { get; set; }
        public string CustomTag { get; set; }
        public long Ranking { get; set; }
    }
}
