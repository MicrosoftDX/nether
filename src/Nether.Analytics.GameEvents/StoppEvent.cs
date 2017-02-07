using System;

namespace Nether.Analytics.GameEvents
{
    public class StoppEvent : IGameEvent
    {
        public string Type => "stop";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string EventCorrelationId { get; set; }
        public string GameSessionId { get; set; }
    }
}