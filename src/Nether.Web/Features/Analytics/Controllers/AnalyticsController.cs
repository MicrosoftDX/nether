// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nether.Data.Analytics;
using Nether.Web.Features.Analytics.Models.Analytics;
using System.Linq;
using System.Net;

namespace Nether.Web.Features.Analytics
{
    [Route("analytics")]
    [Authorize(Roles = RoleNames.Admin)]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsStore _store;
        public AnalyticsController(IAnalyticsStore store)
        {
            _store = store;
        }

        [HttpGet("active-sessions/daily")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ActiveSessionsListResponseModel))]
        public async Task<IActionResult> DailyActiveSessionsAsync()
        {
            var sessions = await _store.GetDailyActiveSessionsAsync();
            var response = new ActiveSessionsListResponseModel
            {
                ActiveSessions = sessions
                                    .Select(s => new ActiveSessionsResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        Day = s.EventDate.Day,
                                        ActiveSessions = s.ActiveSessions
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("active-sessions/monthly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ActiveSessionsListResponseModel))]
        public async Task<IActionResult> MonthlyActiveSessionsAsync()
        {
            var sessions = await _store.GetMonthlyActiveSessionsAsync();
            var response = new ActiveSessionsListResponseModel
            {
                ActiveSessions = sessions
                                    .Select(s => new ActiveSessionsResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        ActiveSessions = s.ActiveSessions
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("active-sessions/yearly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ActiveSessionsListResponseModel))]
        public async Task<IActionResult> YearlyActiveSessionsAsync()
        {
            var sessions = await _store.GetYearlyActiveSessionsAsync();
            var response = new ActiveSessionsListResponseModel
            {
                ActiveSessions = sessions
                                    .Select(s => new ActiveSessionsResponseModel
                                    {
                                        Year = s.Year,
                                        ActiveSessions = s.ActiveSessions
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }



        [HttpGet("active-users/daily")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ActiveUsersListResponseModel))]
        public async Task<IActionResult> DailyActiveUsersAsync()
        {
            var users = await _store.GetDailyActiveUsersAsync();
            var response = new ActiveUsersListResponseModel
            {
                ActiveUsers = users
                                    .Select(s => new ActiveUsersResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        Day = s.EventDate.Day,
                                        ActiveUsers = s.ActiveUsers
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("active-users/monthly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ActiveUsersListResponseModel))]
        public async Task<IActionResult> MonthlyActiveUsersAsync()
        {
            var users = await _store.GetMonthlyActiveUsersAsync();
            var response = new ActiveUsersListResponseModel
            {
                ActiveUsers = users
                                    .Select(s => new ActiveUsersResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        ActiveUsers = s.ActiveUsers
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("active-users/yearly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ActiveUsersListResponseModel))]
        public async Task<IActionResult> YearlyActiveUsersAsync()
        {
            var users = await _store.GetYearlyActiveUsersAsync();
            var response = new ActiveUsersListResponseModel
            {
                ActiveUsers = users
                                    .Select(s => new ActiveUsersResponseModel
                                    {
                                        Year = s.Year,
                                        ActiveUsers = s.ActiveUsers
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }


        [HttpGet("durations/{name}/daily")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DurationsListResponseModel))]
        public async Task<IActionResult> DailyDurationsAsync(string name)
        {
            var durations = await _store.GetDailyDurationsAsync(name);
            var response = new DurationsListResponseModel
            {
                Name = name,
                Durations = durations
                                    .Select(s => new DurationsResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        Day = s.EventDate.Day,
                                        AverageDuration = s.AverageGenericDuration
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("durations/{name}/monthly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DurationsListResponseModel))]
        public async Task<IActionResult> MonthlyDurationsAsync(string name)
        {
            var durations = await _store.GetMonthlyDurationsAsync(name);
            var response = new DurationsListResponseModel
            {
                Name = name,
                Durations = durations
                                    .Select(s => new DurationsResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        AverageDuration = s.AverageGenericDuration
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("durations/{name}/yearly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DurationsListResponseModel))]
        public async Task<IActionResult> YearlyDurationsAsync(string name)
        {
            var durations = await _store.GetYearlyDurationsAsync(name);
            var response = new DurationsListResponseModel
            {
                Name = name,
                Durations = durations
                                    .Select(s => new DurationsResponseModel
                                    {
                                        Year = s.Year,
                                        AverageDuration = s.AverageGenericDuration
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }




        [HttpGet("game-durations/daily")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GameDurationsListResponseModel))]
        public async Task<IActionResult> DailyGameDurationsAsync()
        {
            var durations = await _store.GetDailyGameDurationsAsync();
            var response = new GameDurationsListResponseModel
            {
                Durations = durations
                                    .Select(s => new GameDurationsResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        Day = s.EventDate.Day,
                                        AverageDuration = s.AverageGameDuration
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("game-durations/monthly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GameDurationsListResponseModel))]
        public async Task<IActionResult> MonthlyGameDurationsAsync()
        {
            var durations = await _store.GetMonthlyGameDurationsAsync();
            var response = new GameDurationsListResponseModel
            {
                Durations = durations
                                    .Select(s => new GameDurationsResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        AverageDuration = s.AverageGameDuration
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("game-durations/yearly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GameDurationsListResponseModel))]
        public async Task<IActionResult> YearlyGameDurationsAsync()
        {
            var durations = await _store.GetYearlyGameDurationsAsync();
            var response = new GameDurationsListResponseModel
            {
                Durations = durations
                                    .Select(s => new GameDurationsResponseModel
                                    {
                                        Year = s.Year,
                                        AverageDuration = s.AverageGameDuration
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }


        [HttpGet("level-drop-offs/daily")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LevelDropOffListResponseModel))]
        public async Task<IActionResult> DailyLevelDropOffAsync()
        {
            var dropOffs = await _store.GetDailyLevelDropOffAsync();
            var response = new LevelDropOffListResponseModel
            {
                LevelDropOffs = dropOffs
                                    .Select(s => new LevelDropOffResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        Day = s.EventDate.Day,
                                        ReachedLevel = s.ReachedLevel,
                                        TotalCount = s.TotalCount
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("level-drop-offs/monthly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LevelDropOffListResponseModel))]
        public async Task<IActionResult> MonthlyLevelDropOffAsync()
        {
            var dropOffs = await _store.GetMonthlyLevelDropOffAsync();
            var response = new LevelDropOffListResponseModel
            {
                LevelDropOffs = dropOffs
                                    .Select(s => new LevelDropOffResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        ReachedLevel = s.ReachedLevel,
                                        TotalCount = s.TotalCount
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
        [HttpGet("level-drop-offs/yearly")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LevelDropOffListResponseModel))]
        public async Task<IActionResult> YearlyLevelDropOffAsync()
        {
            var dropOffs = await _store.GetYearlyLevelDropOffAsync();
            var response = new LevelDropOffListResponseModel
            {
                LevelDropOffs = dropOffs
                                    .Select(s => new LevelDropOffResponseModel
                                    {
                                        Year = s.Year,
                                        ReachedLevel = s.ReachedLevel,
                                        TotalCount = s.TotalCount
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }
    }
}