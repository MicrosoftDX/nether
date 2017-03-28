using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetherSDK.Models
{
    [Serializable]
    public class LeaderboardsResult
    {
        public Leaderboards[] leaderboards;
    }

    [Serializable]
    public class Leaderboards
    {
        public string name;
        public string _link;
    }
}
