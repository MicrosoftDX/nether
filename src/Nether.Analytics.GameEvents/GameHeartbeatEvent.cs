using System;

namespace Nether.Analytics.GameEvents
{
    public class GameHeartbeatEvent : IGameEvent
    {
        public string GameEventType => "game-heartbeat";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
    }
}