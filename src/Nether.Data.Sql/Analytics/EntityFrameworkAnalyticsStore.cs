// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.Analytics;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Nether.Data.Sql.Analytics
{
    public class EntityFrameworkAnalyticsStore : IAnalyticsStore
    {
        private AnalyticsContextBase _context;

        private readonly ILogger _logger;

        public EntityFrameworkAnalyticsStore(AnalyticsContextBase context, ILogger<EntityFrameworkAnalyticsStore> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DailyActiveSessionsRecord>> GetDailyActiveSessionsAsync()
        {
            var entities = await _context.DailyActiveSessions
                            .ToListAsync();
            return Map(entities);
        }


        public async Task<IEnumerable<DailyActiveUsersRecord>> GetDailyActiveUsersAsync()
        {
            var entities = await _context.DailyActiveUsers
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<DailyDurationsRecord>> GetDailyDurationsAsync(string name)
        {
            var entities = await _context.DailyDurations
                            .Where(d => d.DisplayName == name)
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<DailyGameDurationsRecord>> GetDailyGameDurationsAsync()
        {
            var entities = await _context.DailyGameDurations
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<DailyLevelDropOffRecord>> GetDailyLevelDropOffAsync()
        {
            var entities = await _context.DailyLevelDropOff
                .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<MonthlyActiveSessionsRecord>> GetMonthlyActiveSessionsAsync()
        {
            var entities = await _context.MonthlyActiveSessions
                            .ToListAsync();
            return Map(entities);
        }


        public async Task<IEnumerable<MonthlyActiveUsersRecord>> GetMonthlyActiveUsersAsync()
        {
            var entities = await _context.MonthlyActiveUsers
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<MonthlyDurationsRecord>> GetMonthlyDurationsAsync(string name)
        {
            var entities = await _context.MonthlyDurations
                            .Where(d => d.DisplayName == name)
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<MonthlyGameDurationsRecord>> GetMonthlyGameDurationsAsync()
        {
            var entities = await _context.MonthlyGameDurations
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<MonthlyLevelDropOffRecord>> GetMonthlyLevelDropOffAsync()
        {
            var entities = await _context.MonthlyLevelDropOff
                .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<YearlyActiveSessionsRecord>> GetYearlyActiveSessionsAsync()
        {
            var entities = await _context.YearlyActiveSessions
                            .ToListAsync();
            return Map(entities);
        }


        public async Task<IEnumerable<YearlyActiveUsersRecord>> GetYearlyActiveUsersAsync()
        {
            var entities = await _context.YearlyActiveUsers
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<YearlyDurationsRecord>> GetYearlyDurationsAsync(string name)
        {
            var entities = await _context.YearlyDurations
                            .Where(d => d.DisplayName == name)
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<YearlyGameDurationsRecord>> GetYearlyGameDurationsAsync()
        {
            var entities = await _context.YearlyGameDurations
                            .ToListAsync();
            return Map(entities);
        }

        public async Task<IEnumerable<YearlyLevelDropOffRecord>> GetYearlyLevelDropOffAsync()
        {
            var entities = await _context.YearlyLevelDropOff
                .ToListAsync();
            return Map(entities);
        }


        // TODO - bring in automapper? :-)
        private IEnumerable<DailyActiveSessionsRecord> Map(List<DailyActiveSessionsEntity> entities)
        {
            return entities.Select(e =>
                        new DailyActiveSessionsRecord
                        {
                            EventDate = e.EventDate,
                            ActiveSessions = e.ActiveSessions
                        })
                .ToList();
        }
        private IEnumerable<MonthlyActiveSessionsRecord> Map(List<MonthlyActiveSessionsEntity> entities)
        {
            return entities.Select(e =>
                        new MonthlyActiveSessionsRecord
                        {
                            EventDate = e.EventDate,
                            ActiveSessions = e.ActiveSessions
                        })
                .ToList();
        }
        private IEnumerable<YearlyActiveSessionsRecord> Map(List<YearlyActiveSessionsEntity> entities)
        {
            return entities.Select(e =>
                        new YearlyActiveSessionsRecord
                        {
                            Year = e.Year,
                            ActiveSessions = e.ActiveSessions
                        })
                .ToList();
        }

        private IEnumerable<DailyActiveUsersRecord> Map(List<DailyActiveUsersEntity> entities)
        {
            return entities.Select(e =>
                        new DailyActiveUsersRecord
                        {
                            EventDate = e.EventDate,
                            ActiveUsers = e.ActiveUsers
                        })
                .ToList();
        }
        private IEnumerable<MonthlyActiveUsersRecord> Map(List<MonthlyActiveUsersEntity> entities)
        {
            return entities.Select(e =>
                        new MonthlyActiveUsersRecord
                        {
                            EventDate = e.EventDate,
                            ActiveUsers = e.ActiveUsers
                        })
                .ToList();
        }
        private IEnumerable<YearlyActiveUsersRecord> Map(List<YearlyActiveUsersEntity> entities)
        {
            return entities.Select(e =>
                        new YearlyActiveUsersRecord
                        {
                            Year = e.Year,
                            ActiveUsers = e.ActiveUsers
                        })
                .ToList();
        }

        private IEnumerable<DailyDurationsRecord> Map(List<DailyDurationsEntity> entities)
        {
            return entities.Select(e =>
                        new DailyDurationsRecord
                        {
                            EventDate = e.EventDate,
                            DisplayName = e.DisplayName,
                            AverageGenericDuration = e.AverageGenericDuration
                        })
                .ToList();
        }
        private IEnumerable<MonthlyDurationsRecord> Map(List<MonthlyDurationsEntity> entities)
        {
            return entities.Select(e =>
                        new MonthlyDurationsRecord
                        {
                            EventDate = e.EventDate,
                            DisplayName = e.DisplayName,
                            AverageGenericDuration = e.AverageGenericDuration
                        })
                .ToList();
        }
        private IEnumerable<YearlyDurationsRecord> Map(List<YearlyDurationsEntity> entities)
        {
            return entities.Select(e =>
                        new YearlyDurationsRecord
                        {
                            Year = e.Year,
                            DisplayName = e.DisplayName,
                            AverageGenericDuration = e.AverageGenericDuration
                        })
                .ToList();
        }


        private IEnumerable<DailyGameDurationsRecord> Map(List<DailyGameDurationsEntity> entities)
        {
            return entities.Select(e =>
                        new DailyGameDurationsRecord
                        {
                            EventDate = e.EventDate,
                            AverageGameDuration = e.AverageGameDuration
                        })
                .ToList();
        }
        private IEnumerable<MonthlyGameDurationsRecord> Map(List<MonthlyGameDurationsEntity> entities)
        {
            return entities.Select(e =>
                        new MonthlyGameDurationsRecord
                        {
                            EventDate = e.EventDate,
                            AverageGameDuration = e.AverageGameDuration
                        })
                .ToList();
        }
        private IEnumerable<YearlyGameDurationsRecord> Map(List<YearlyGameDurationsEntity> entities)
        {
            return entities.Select(e =>
                        new YearlyGameDurationsRecord
                        {
                            Year = e.Year,
                            AverageGameDuration = e.AverageGameDuration
                        })
                .ToList();
        }


        private IEnumerable<DailyLevelDropOffRecord> Map(List<DailyLevelDropOffEntity> entities)
        {
            return entities.Select(e =>
                        new DailyLevelDropOffRecord
                        {
                            EventDate = e.EventDate,
                            ReachedLevel = e.ReachedLevel,
                            TotalCount = e.TotalCount
                        })
                .ToList();
        }
        private IEnumerable<MonthlyLevelDropOffRecord> Map(List<MonthlyLevelDropOffEntity> entities)
        {
            return entities.Select(e =>
                        new MonthlyLevelDropOffRecord
                        {
                            EventDate = e.EventDate,
                            ReachedLevel = e.ReachedLevel,
                            TotalCount = e.TotalCount
                        })
                .ToList();
        }
        private IEnumerable<YearlyLevelDropOffRecord> Map(List<YearlyLevelDropOffEntity> entities)
        {
            return entities.Select(e =>
                        new YearlyLevelDropOffRecord
                        {
                            Year = e.Year,
                            ReachedLevel = e.ReachedLevel,
                            TotalCount = e.TotalCount
                        })
                .ToList();
        }
    }
}
