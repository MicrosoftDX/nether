using System;

namespace Nether.Analytics.GameEvents
{
    public class GameStopEvent : IGameEvent
    {
        public string Type => "game-stop";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
    }
}