using System;
using System.Collections.Generic;

namespace Nether.Analytics.EventProcessor.GameEvents
{
    public interface IOutGameEvent
    {
        string Type { get; }
        string Version { get; }
        DateTime EnqueTime { get; set; }
        DateTime DequeTime { get; set; }
        DateTime ClientUtcTime { get; set; }
        Dictionary<string, string> Properties { get; }
    }
}
