using System.Collections.Generic;

namespace Nether.Leaderboard.Web.Controllers
{
    public class ScoresListResponeModel<ScoreResponeModel>
    {
        public List<ScoreResponeModel> Leaderboard { get; set; }
    }
}