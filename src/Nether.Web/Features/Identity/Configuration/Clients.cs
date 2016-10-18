using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Configuration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "devclient",
                    ClientName = "Dev Client",
                    AllowedGrantTypes = GrantTypes.Hybrid.Concat(new [] { "fb-usertoken" }),

                    RedirectUris = new List<string>
                    {
                        "http://localhost:5000/signin-oidc"
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:5000/"
                    },
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("devsecret".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        "nether-all"
                    }
                }
            };
        }
    }
}
