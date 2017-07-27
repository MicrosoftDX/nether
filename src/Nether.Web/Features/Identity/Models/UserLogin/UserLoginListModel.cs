using System.Collections.Generic;

namespace Nether.Web.Features.Identity.Models.UserLogin
{
    public class UserLoginListModel
    {
        public IEnumerable<UserLoginModel> Logins { get; set; }
    }
}
