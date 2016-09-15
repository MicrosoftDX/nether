using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

using Nether.Data.Leaderboard;
using System.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Web.Features.Leaderboard
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
        public async Task<ActionResult> Get() //TODO: add swagger annotations for response shape
        {
            // Call data store
            var scores = await _store.GetAllHighScoresAsync();

            // Format response model
            var resultModel = new LeaderboardGetResponseModel
            {
                LeaderboardEntries = scores.Select(s => (LeaderboardGetResponseModel.LeaderboardEntry)s).ToList()
            };

            // Return result
            return Ok(resultModel);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]LeaderboardPostRequestModel score)
        {
            //TODO: Make validation more sofisticated, perhaps some games want/need negative scores
            // Validate input
            if (score.Score < 0)
            {
                return StatusCode((int)HttpStatusCode.BadRequest); //TODO: return error info in body
            }

            // Call data store
            await _store.SaveScoreAsync(new GameScore {Gamertag = score.Gamertag, Score = score.Score});

            // Return result
            return Ok();
        }
    }
}
