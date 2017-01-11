using IdentityModel;
using Nether.Data.Identity;
using Nether.Integration.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity
{
    public class UserClaimsProvider
    {
        private readonly IIdentityPlayerManagementClient _playerManagementClient;
        public UserClaimsProvider(IIdentityPlayerManagementClient playerManagementClient)
        {
            _playerManagementClient = playerManagementClient;
        }
        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserId),
                new Claim(JwtClaimTypes.Name, user.UserId),
                new Claim(JwtClaimTypes.Role, user.Role),
            };

            var gamertag = await _playerManagementClient.GetGamertagForUserIdAsync(user.UserId);
            if (!string.IsNullOrEmpty(gamertag))
            {
                claims.Add(new Claim(JwtClaimTypes.NickName, gamertag));
            }

            return claims;
        }

    }
}
