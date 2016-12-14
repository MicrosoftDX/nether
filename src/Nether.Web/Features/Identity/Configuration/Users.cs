// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
                        new Claim(JwtClaimTypes.Role, RoleNames.Player),
                    }
                },
                new InMemoryUser{Subject = "2222", Username = "devadmin", Password = "devadmin",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "devadmin"),
                        new Claim(JwtClaimTypes.GivenName, "Development"),
                        new Claim(JwtClaimTypes.FamilyName, "Admin"),
                        new Claim(JwtClaimTypes.Role, RoleNames.Admin),
                    }
                },

                // Integration test users
                // user without gamertag
                new InMemoryUser{Subject = "112299", Username = "testuser-notag", Password = "password123",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "112299"),
                        new Claim(JwtClaimTypes.Role, RoleNames.Player),
                    }
                },
                // user with gamertag
                new InMemoryUser{Subject = "3333", Username = "testuser", Password = "testuser",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "testuser"),
                        new Claim(JwtClaimTypes.GivenName, "Development"),
                        new Claim(JwtClaimTypes.FamilyName, "User"),
                        new Claim(JwtClaimTypes.Role, RoleNames.Player)
                    }
                },
                new InMemoryUser{Subject = "4444", Username = "testuser1", Password = "testuser1",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "testuser1"),
                        new Claim(JwtClaimTypes.GivenName, "Development"),
                        new Claim(JwtClaimTypes.FamilyName, "User"),
                        new Claim(JwtClaimTypes.Role, RoleNames.Player),
                    }
                },
                new InMemoryUser{Subject = "5555", Username = "testuser2", Password = "testuser2",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "testuser2"),
                        new Claim(JwtClaimTypes.GivenName, "Development"),
                        new Claim(JwtClaimTypes.FamilyName, "User"),
                        new Claim(JwtClaimTypes.Role, RoleNames.Player),
                    }
                },
                new InMemoryUser{Subject = "6666", Username = "testuser3", Password = "testuser3",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "testuser3"),
                        new Claim(JwtClaimTypes.GivenName, "Development"),
                        new Claim(JwtClaimTypes.FamilyName, "User"),
                        new Claim(JwtClaimTypes.Role, RoleNames.Player),
                    }
                }
            };


        public static List<InMemoryUser> Get()
        {
            var result = new List<InMemoryUser>();
            result.AddRange(Value);
            result.AddRange(GenerateForLoadTest(10000));
            return result;
        }

        private static List<InMemoryUser> GenerateForLoadTest(int count)
        {
            return Enumerable.Range(0, count)
                .Select(i => new InMemoryUser
                {
                    Subject = $"subj{i}",
                    Username = $"loadUser{i}",
                    Password = $"loadUser{i}",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, $"loadUser{i}"),
                        new Claim(JwtClaimTypes.GivenName, "Load Test"),
                        new Claim(JwtClaimTypes.FamilyName, $"User #{i}"),
                        new Claim(JwtClaimTypes.Role, RoleNames.Player),
                    }
                })
                .ToList();
        }
    }
}
