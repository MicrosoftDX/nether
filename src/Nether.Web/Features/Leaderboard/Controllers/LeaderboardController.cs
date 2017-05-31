// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Nether.Data.Leaderboard;
using Nether.Integration.Analytics;
using Nether.Web.Features.Leaderboard.Configuration;
using Nether.Web.Utilities;
using System.Net;
using Microsoft.Extensions.Logging;
using Nether.Web.Features.Leaderboard.Models.Leaderboard;
using Nether.Common.ApplicationPerformanceMonitoring;
using Nether.Integration.Leaderboard;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Nether.Web.Features.Leaderboard
{
    /// <summary>
    /// Leaderboard management
    /// </summary>
    [Route("leaderboards")]
    [NetherService("Leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardStore _store;
        private readonly ILogger<LeaderboardController> _logger;
        private readonly IApplicationPerformanceMonitor _appMonitor;

        private readonly ILeaderboardProvider _leaderboardProvider;
        private readonly ILeaderboardPlayerManagementClient _playerManagementClient;

        public LeaderboardController(
            ILeaderboardStore store,
            ILogger<LeaderboardController> logger,
            IApplicationPerformanceMonitor appMonitor,
            ILeaderboardProvider leaderboardProvider,
            ILeaderboardPlayerManagementClient playerManagementClient
            )
        {
            _store = store;
            _logger = logger;
            _appMonitor = appMonitor;
            _leaderboardProvider = leaderboardProvider;
            _playerManagementClient = playerManagementClient;
        }

        /// <summary>
        /// Gets leaderboard by leaderboard configured name
        /// </summary>
        /// <returns>List of scores and gametags</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LeaderboardListResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [Authorize(Roles = RoleNames.PlayerOrAdmin)]
        [HttpGet("")]
        public IActionResult GetAll()
        {
            var leaderboards = _leaderboardProvider.GetAll();

            return Ok(new LeaderboardListResponseModel
            {
                Leaderboards = leaderboards
                                .Select(l => new LeaderboardListResponseModel.LeaderboardSummaryModel
                                {
                                    Name = l.Name,
                                    _Link = Url.RouteUrl(nameof(Get), new { name = l.Name })
                                })
                                .ToList()
            });
        }

        /// <summary>
        /// Gets leaderboard by leaderboard configured name
        /// </summary>
        /// <param name="name">Name of the leaderboard</param>
        /// <returns>List of scores and gametags</returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LeaderboardGetResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Authorize(Roles = RoleNames.PlayerOrAdmin)]
        [HttpGet("{name}", Name = nameof(Get))]
        public async Task<IActionResult> Get(string name)
        {
            var userId = User.GetId();

            _appMonitor.LogEvent("Leaderboard", properties: new Dictionary<string, string> {
                { "Name", name }
            });

            LeaderboardConfig config = _leaderboardProvider.GetByName(name);
            if (config == null)
            {
                return NotFound();
            }
            LeaderboardType type = config.Type;
            List<GameScore> scores;
            switch (type)
            {
                case LeaderboardType.AroundMe:
                    if (userId == null)
                    {
                        return this.ValidationFailed(new ErrorDetail("userId", "Must be signed in as a player with a userId to retrive this leaderboard"));
                    }
                    scores = await _store.GetScoresAroundUserAsync(userId, config.Radius);
                    break;
                case LeaderboardType.Top:
                    scores = await _store.GetTopHighScoresAsync(config.Top);
                    break;
                default:
                    scores = await _store.GetAllHighScoresAsync();
                    break;
            }
            var userIdsToLookUp = scores.Select(s => s.UserId).ToList();

            GameScore currentPlayer = null;
            if (config.IncludeCurrentPlayer && userId != null)
            {
                currentPlayer = (await _store.GetScoresAroundUserAsync(userId, 0)).FirstOrDefault();
                if (currentPlayer != null && !userIdsToLookUp.Contains(currentPlayer.UserId))
                {
                    userIdsToLookUp.Add(currentPlayer.UserId);
                }
            }

            var gamertags = await _playerManagementClient.GetGamertagsForUserIdsAsync(userIdsToLookUp.ToArray());

            var resultModel = new LeaderboardGetResponseModel
            {
                Entries = scores
                            ?.Select(s => Map(s, userId))
                            ?.ToList(),
                CurrentPlayer = Map(currentPlayer, null)
            };

            return Ok(resultModel);

            // local function to map to response types
            LeaderboardGetResponseModel.LeaderboardEntry Map(GameScore score, string currentUserId)
            {
    if (score == null)
        return null;

    //find gamertag for userid

    return new LeaderboardGetResponseModel.LeaderboardEntry
    {
        Gamertag = gamertags.Single(g => g.UserId == score.UserId).Gamertag,
        Score = score.Score,
        Rank = score.Rank,
        IsCurrentPlayer = currentUserId == score.UserId
    };
}
        }
    }
}

