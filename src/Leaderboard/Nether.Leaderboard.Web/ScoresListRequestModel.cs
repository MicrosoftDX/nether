using System.Collections.Generic;

namespace Nether.Leaderboard.Web.Controllers
{
    public class ScoresListRequestModel<ScoreRequestModel>
    {
        public List<ScoreRequestModel> Leaderboard { get; set; }
    }
}