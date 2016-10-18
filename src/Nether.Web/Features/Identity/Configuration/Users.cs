using IdentityModel;
using IdentityServer4.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Configuration
{
    public class Users
    {
        public static List<InMemoryUser> Value = new List<InMemoryUser>
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
        public static List<InMemoryUser> Get()
        {
            return Value;
        }
    }
}
