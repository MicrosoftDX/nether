using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Nether.Web.Features.Analytics.Models.Analytics
{
    public class GameDurationsListResponseModel
    {
        public List<GameDurationsResponseModel> Durations { get; set; }
    }

    public class GameDurationsResponseModel
    {
        public int Year { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public int? Month { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public int? Day { get; set; }

        public long AverageDuration { get; set; }
    }
}