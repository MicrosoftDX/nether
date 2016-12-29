// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;

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
                    AllowedGrantTypes = GrantTypes.Hybrid
                                            .Concat(GrantTypes.ResourceOwnerPassword)
                                            .Concat(new [] { "fb-usertoken" }),

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
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        "nether-all"
                    }
                },

                new Client
                {
                    ClientId = "clientcreds-test",
                    ClientName = "Test Client for client credentials flow",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("devsecret".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        "nether-all"
                    }
                },
                new Client
                {
                    ClientId = "resourceowner-test",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("devsecret".Sha256())
                    },
                    AllowedScopes = { "nether-all" }
                },
                new Client
                {
                    ClientId = "customgrant-test",
                    AllowedGrantTypes = GrantTypes.List("fb-usertoken"),

                    ClientSecrets =
                    {
                        new Secret("devsecret".Sha256())
                    },
                    AllowedScopes = { "nether-all" }
                },

                //implicit flow client (angular 2 application)
                new Client
                {
                    ClientName = "angular2client",
                    ClientId = "angular2client",
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:5000"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:5000/login"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:5000",
                        "https://localhost:5000"
                    },
                    AllowedScopes = new List<string>
                    {
                        "nether-all"
                    }
                }
            };
        }
    }
}
