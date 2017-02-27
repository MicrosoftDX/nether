using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Nether.Web.Features.Analytics.Models.Analytics
{
    public class ActiveUsersListResponseModel
    {
        public List<ActiveUsersResponseModel> ActiveUsers { get; set; }
    }

    public class ActiveUsersResponseModel
    {
        public int Year { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public int? Month { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public int? Day { get; set; }

        public int ActiveUsers { get; set; }
    }
}