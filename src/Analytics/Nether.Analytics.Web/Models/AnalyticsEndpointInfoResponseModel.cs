using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Analytics.Web.Models
{
    public class AnalyticsEndpointInfoResponseModel
    {
        public string HttpVerb { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public string Authorization { get; set; }
        public DateTime ValidUntilUtc { get; set; }
    }
}
