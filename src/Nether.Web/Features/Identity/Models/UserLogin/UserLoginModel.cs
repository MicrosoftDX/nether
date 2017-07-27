using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Models.UserLogin
{
    public class UserLoginModel
    {
        public string ProviderType { get; set; }
        public string ProviderId { get; set; }
    }
}
