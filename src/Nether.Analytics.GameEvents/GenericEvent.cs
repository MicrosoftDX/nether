using System;
using System.Collections.Generic;

namespace Nether.Analytics.GameEvents
{
    public class GenericEvent : IGameEvent
    {
        public string Type => "generic";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}