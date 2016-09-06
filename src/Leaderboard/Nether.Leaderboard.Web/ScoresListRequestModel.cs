using System.Collections.Generic;

namespace Nether.Leaderboard.Web.Controllers
{
    public class ScoresListRequestModel<T>
    {
        public List<ScoreRequestModel> Leaderboard { get; set; }
    }
}