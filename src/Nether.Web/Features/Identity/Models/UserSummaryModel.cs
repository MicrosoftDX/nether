using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Models
{
    public class UserSummaryModel
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public string _Link { get; set; }
    }
}
