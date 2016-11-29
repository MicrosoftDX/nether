using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nether.Web.Utilities
{
    public static class UserExtensions
    {
        public static string GetGamerTag(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName)?.Value;
        }
    }
}
