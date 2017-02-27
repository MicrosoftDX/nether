using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Nether.Web.Features.Analytics.Models.Analytics
{
    public class ActiveSessionsListResponseModel
    {
        public List<ActiveSessionsResponseModel> ActiveSessions { get; set; }
    }

    public class ActiveSessionsResponseModel
    {
        public int Year { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public int? Month { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public int? Day { get; set; }

        public int ActiveSessions { get; set; }
    }
}