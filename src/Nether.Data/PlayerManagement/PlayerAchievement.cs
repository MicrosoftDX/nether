// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.Achievement;

namespace Nether.Data.PlayerManagement
{
    public class PlayerAchievement
    {
        public string AchievementId { get; set; }
        public string PlayerId { get; set; }
        public State CurrentState { get; set; }
        public int Percentage { get; set; }
    }
}
