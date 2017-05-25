
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.PlayerManagement.Models.PlayerManagementIntegration
{
    public class GamertagsLookupRequestModel
    {
        [Required]
        public string[] UserIds { get; set; }
    }
}
