using System;

namespace Nether.Analytics.GameEvents
{
    public interface IGameEvent
    {
        string GameEventType { get; }
        string Version { get; }
        DateTime ClientUtcTime { get; set; }
    }
}