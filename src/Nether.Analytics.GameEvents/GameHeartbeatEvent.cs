using System;

namespace Nether.Analytics.GameEvents
{
    public class GameHeartbeatEvent : IGameEvent
    {
        public string Type => "game-heartbeat";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
    }
}