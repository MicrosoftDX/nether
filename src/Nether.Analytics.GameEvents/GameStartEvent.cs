using System;

namespace Nether.Analytics.GameEvents
{
    public class GameStartEvent : IGameEvent
    {
        public string Type => "game-start";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public string GamerTag { get; set; }
    }
}
