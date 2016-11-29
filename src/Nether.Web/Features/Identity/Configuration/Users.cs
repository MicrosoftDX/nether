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
                        new Claim(JwtClaimTypes.Role, "player"),
                    }
                },
                new InMemoryUser{Subject = "2222", Username = "devadmin", Password = "devadmin",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "devadmin"),
                        new Claim(JwtClaimTypes.GivenName, "Development"),
                        new Claim(JwtClaimTypes.FamilyName, "Admin"),
                        new Claim(JwtClaimTypes.Role, "admin"),
                    }
                },


                // Integration test users
                // user without gamertag
                new InMemoryUser{Subject = "112299", Username = "testuser-notag", Password = "password123",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "112299"),
                        new Claim(JwtClaimTypes.Role, "player"),
                    }
                },
                // user with gamertag
                new InMemoryUser{Subject = "112233", Username = "testuser", Password = "password123",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "112233"),
                        new Claim(JwtClaimTypes.Role, "player"),
                        new Claim(JwtClaimTypes.NickName, "testusertag") // gamertag
                    }
                },
};
        public static List<InMemoryUser> Get()
        {
            return Value;
        }
    }
}
