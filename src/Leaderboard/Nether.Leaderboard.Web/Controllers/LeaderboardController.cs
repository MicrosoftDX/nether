using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Nether.Leaderboard.Data;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Leaderboard.Web.Controllers
{
    [Route("api/leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardStore _store;

        public LeaderboardController(ILeaderboardStore store)
        {
            _store = store;
        }

        //TODO: Add versioning support
        //TODO: Add authentication


        [HttpGet]
        public async Task<ActionResult> Get() // TODO add swagger annotations for response shape
        {
            var scores = await _store.GetScoresAsync();
            var resultModel = new ScoresListRequestModel<ScoreRequestModel>
            {
                Leaderboard = ToScoresModel(scores)
            };
            return Ok(resultModel);
        }

        private List<ScoreRequestModel> ToScoresModel(Dictionary<string, int> scores)
        {
            List<ScoreRequestModel> result = new List<ScoreRequestModel>();
            foreach (var item in scores)
            {
                result.Add(new ScoreRequestModel
                {
                    Gamertag = item.Key,
                    Score = item.Value
                });
            }
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]ScoreRequestModel score)
        {
            if (score.Score < 0)
            {
                return StatusCode((int)HttpStatusCode.BadRequest); // TODO return error info in body
            }

            await _store.SaveScoreAsync(score.Gamertag, score.Score);
            return Ok();
        }
    }
}
