// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Nether.Integration.Analytics
{
    public interface IAnalyticsIntegrationClient
    {
        Task SendGameEventAsync(GameEvent gameEvent);
    }

    public abstract class GameEvent
    {
        public DateTime UtcDateTime { get; set; }
        public string GamerTag { get; set; }
        public abstract string EventType { get; }
        public abstract string Version { get; }
    }

    public class HeartBeatGameEvent : GameEvent
    {
        public override string EventType => "HeartBeat";
        public override string Version => "1.0.0";
    }

    public class SessionStartedGameEvent : GameEvent
    {
        public override string EventType => "SessionStarted";
        public override string Version => "1.0.0";
    }

    public class SessionEndedGameEvent : GameEvent
    {
        public override string EventType => "SessionEnded";
        public override string Version => "1.0.0";
    }

    public class ScoreAchieved : GameEvent
    {
        public override string EventType => "ScoreAchieved";
        public override string Version => "1.0.0";
        public int Score { get; set; }
    }
}

