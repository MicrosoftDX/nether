using System;

namespace Nether.Analytics.GameEvents
{
    public class ScoreEvent : IGameEvent
    {
        public string Type => "score";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public long Score { get; set; }
    }
}