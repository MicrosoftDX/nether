using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.Leaderboard
{
    public class SavedGamerScore
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string GamerTag { get; set; }
        public string CustomTag { get; set; }
        public DateTime DateAchieved { get; set; }
    }
}
