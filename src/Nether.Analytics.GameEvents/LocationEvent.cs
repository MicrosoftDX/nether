using System;

namespace Nether.Analytics.GameEvents
{
    public class LocationEvent : IGameEvent
    {
        public string Type => "location";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
    }
}