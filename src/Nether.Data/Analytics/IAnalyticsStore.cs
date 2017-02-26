// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nether.Data.Analytics
{
    public interface IAnalyticsStore
    {
        // TODO Need to add paging/filtering capabilities to these methods
        // TODO Revisit how these are stored to consider ways to collapse this into more generic methods

        Task<IEnumerable<DailyActiveSessionsRecord>> GetDailyActiveSessionsAsync();
        Task<IEnumerable<MonthlyActiveSessionsRecord>> GetMonthlyActiveSessionsAsync();
        Task<IEnumerable<YearlyActiveSessionsRecord>> GetYearlyActiveSessionsAsync();

        Task<IEnumerable<DailyActiveUsersRecord>> GetDailyActiveUsersAsync();
        Task<IEnumerable<MonthlyActiveUsersRecord>> GetMonthlyActiveUsersAsync();
        Task<IEnumerable<YearlyActiveUsersRecord>> GetYearlyActiveUsersAsync();

        Task<IEnumerable<DailyDurationsRecord>> GetDailyDurationsAsync();
        Task<IEnumerable<MonthlyDurationsRecord>> GetMonthlyDurationsAsync();
        Task<IEnumerable<YearlyDurationsRecord>> GetYearlyDurationsAsync();

        Task<IEnumerable<DailyGameDurationsRecord>> GetDailyGameDurationsAsync();
        Task<IEnumerable<MonthlyGameDurationsRecord>> GetMonthlyGameDurationsAsync();
        Task<IEnumerable<YearlyGameDurationsRecord>> GetYearlyGameDurationsAsync();

        Task<IEnumerable<DailyLevelDropOffRecord>> GetDailyLevelDropOffAsync();
        Task<IEnumerable<MonthlyLevelDropOffRecord>> GetMonthlyLevelDropOffAsync();
        Task<IEnumerable<YearlyLevelDropOffRecord>> GetYearlyLevelDropOffAsync();
    }
}

