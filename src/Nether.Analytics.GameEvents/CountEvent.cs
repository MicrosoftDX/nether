using System;

namespace Nether.Analytics.GameEvents
{
    public class CountEvent : IGameEvent
    {
        public string Type => "count";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string DisplayName { get; set; }
        public int Value { get; set; }
        public string GameSessionId { get; set; }
    }
}