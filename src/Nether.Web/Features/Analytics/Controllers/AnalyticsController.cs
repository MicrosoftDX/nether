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

        [HttpGet("/active-sessions/daily")]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(ActiveSessionsResponseModel))]
        public async Task<IActionResult> DailyActiveSessionsAsync()
        {
            var sessions = await _store.GetDailyActiveSessionsAsync();
            var response = new ActiveSessionsResponseModel
            {
                ActiveSessions = sessions
                                    .Select(s => new ActiveSessionResponseModel
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

        [HttpGet("/active-sessions/monthly")]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(ActiveSessionsResponseModel))]
        public async Task<IActionResult> MonthlyActiveSessionsAsync()
        {
            var sessions = await _store.GetMonthlyActiveSessionsAsync();
            var response = new ActiveSessionsResponseModel
            {
                ActiveSessions = sessions
                                    .Select(s => new ActiveSessionResponseModel
                                    {
                                        Year = s.EventDate.Year,
                                        Month = s.EventDate.Month,
                                        ActiveSessions = s.ActiveSessions
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }


        [HttpGet("/active-sessions/yearly")]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(ActiveSessionsResponseModel))]
        public async Task<IActionResult> YearlyActiveSessionsAsync()
        {
            var sessions = await _store.GetYearlyActiveSessionsAsync();
            var response = new ActiveSessionsResponseModel
            {
                ActiveSessions = sessions
                                    .Select(s => new ActiveSessionResponseModel
                                    {
                                        Year = s.Year,
                                        ActiveSessions = s.ActiveSessions
                                    })
                                    .ToList()
            };
            return base.Ok(response);
        }

    }
}