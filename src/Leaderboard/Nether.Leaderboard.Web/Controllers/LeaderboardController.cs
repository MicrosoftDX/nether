using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Nether.Leaderboard.Data;

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
            var score = await _store.GetScoreAsync("anonymous");
            return Ok(score);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]int score)
        {
            if (score < 0)
            {
                return StatusCode((int)HttpStatusCode.BadRequest); // TODO return error info in body
            }

            await _store.SaveScoreAsync("anonymous", score);
            return Ok();
        }
    }
}
