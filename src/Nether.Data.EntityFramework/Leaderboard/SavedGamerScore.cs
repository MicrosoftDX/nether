// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.EntityFramework.Leaderboard
{
    public class SavedGamerScore
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string UserId { get; set; }
        public DateTime DateAchieved { get; set; }
    }
}
