using System;
using System.Collections.Generic;
using System.Text;

namespace Nether.Analytics.MessageFormats
{
    class SessionStart : INetherMessage
    {
        public string Type => "session-start";
        public string Version => "1.0.0";
        //public DateTime ClientUtcTime { get; set; }
        public string GameSession { get; set; }
        public string GamerTag { get; set; }
    }
}
