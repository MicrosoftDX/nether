using System;

namespace Nether.Analytics.GameEvents
{
    public class StartEvent : IGameEvent
    {
        public string Type => "start";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string EventCorrelationId { get; set; }
        public string DisplayName { get; set; }
        public string GameSessionId { get; set; }
    }
}