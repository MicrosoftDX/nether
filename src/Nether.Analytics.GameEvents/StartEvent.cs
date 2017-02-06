using System;

namespace Nether.Analytics.GameEvents
{
    public class StartEvent : IGameEvent
    {
        public string GameEventType => "start";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public string EventCorrelationId { get; set; }
    }
}