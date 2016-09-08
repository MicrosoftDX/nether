using IdentityModel;
using IdentityServer4.Quickstart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nether.Identity.Development.Configuration
{
    public class Users
    {
        public static List<InMemoryUser> Get()
        {
            var users = new List<InMemoryUser>
            {
                new InMemoryUser{Subject = "1111", Username = "devuser", Password = "devuser",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "devuser"),
                        new Claim(JwtClaimTypes.GivenName, "Development"),
                        new Claim(JwtClaimTypes.FamilyName, "User"),
                        new Claim(JwtClaimTypes.Role, "Player"),
                    }
                },
                new InMemoryUser{Subject = "2222", Username = "devadmin", Password = "devadmin",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "devadmin"),
                        new Claim(JwtClaimTypes.GivenName, "Development"),
                        new Claim(JwtClaimTypes.FamilyName, "Admin"),
                        new Claim(JwtClaimTypes.Role, "Admin"),
                    }
                },
            };

            return users;
        }
    }
}
