using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    //TODO: Move file out of here as soon as we find a good way of sharing the Game Event Types between different projects.
    // Right now this is a copy of how the event type look like in the Nether.Analytics.GameEvents 
    public class IncommingLocationEvent
    {
        public string Type => "location";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

    public class OutgoingLocationEvent : IOutGameEvent
    {
        public string Type => "location";
        public string Version => "1.0.0";
        public DateTime EnqueTime { get; set; }
        public DateTime DequeTime { get; set; }
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public long GeoHash { get; set; }
        public int GeoHashPrecision { get; set; }
        public double GeoHashCenterLat { get; set; }
        public double GeoHashCenterLon { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
