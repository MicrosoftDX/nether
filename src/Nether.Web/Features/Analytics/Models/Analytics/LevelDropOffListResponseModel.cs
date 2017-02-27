using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Nether.Web.Features.Analytics.Models.Analytics
{
    public class LevelDropOffListResponseModel
    {
        public List<LevelDropOffResponseModel> LevelDropOffs { get; set; }
    }

    public class LevelDropOffResponseModel
    {
        public int Year { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public int? Month { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public int? Day { get; set; }

        public int ReachedLevel { get; set; }
        public long TotalCount { get; set; }
    }
}