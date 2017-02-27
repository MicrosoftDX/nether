using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Nether.Web.Features.Analytics.Models.Analytics
{
    public class DurationsListResponseModel
    {
        public List<DurationsResponseModel> Durations { get; set; }
        public string Name { get; set; }
    }

    public class DurationsResponseModel
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