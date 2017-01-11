// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Nether.Web.Features.Identity.Configuration
{
    // TODO - remove this class!!
    //      - need to provide a way to initialise the users on a clean install
    //      - need to provide a way to initialise the users for a load test
    //      - need to provide a way to initialise the users for an integration test
    public class InMemoryUsersSeed
    {
        public static List<User> Get(IPasswordHasher passwordHasher, bool includeLoadTestUsers = true)
        {
            var result = new List<User>
            {
                new User {
                    UserId = "1111",
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            ProviderType = LoginProvider.UserNamePassword,
                            ProviderId = "devuser",
                            ProviderData = passwordHasher.HashPassword("devuser")
                        }
                    }
                },
                new User {
                    UserId = "2222",
                    Role = RoleNames.Admin,
                    IsActive = true,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            ProviderType = LoginProvider.UserNamePassword,
                            ProviderId = "devadmin",
                            ProviderData = passwordHasher.HashPassword("devadmin")
                        }
                    }
                },
                // Integration test users
                // user without gamertag
                new User{
                    UserId =  "112299",
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            ProviderType = LoginProvider.UserNamePassword,
                            ProviderId = "testuser-notag",
                            ProviderData = passwordHasher.HashPassword("password123")
                        }
                    }
                },
                // user with gamertag
                new User {
                    UserId =  "3333",
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            ProviderType = LoginProvider.UserNamePassword,
                            ProviderId = "testuser",
                            ProviderData = passwordHasher.HashPassword("testuser")
                        }
                    }
                },
                new User{
                    UserId =  "4444",
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            ProviderType = LoginProvider.UserNamePassword,
                            ProviderId = "testuser1",
                            ProviderData = passwordHasher.HashPassword("testuser1")
                        }
                    }
                },
                new User{
                    UserId =  "5555",
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            ProviderType = LoginProvider.UserNamePassword,
                            ProviderId = "testuser2",
                            ProviderData = passwordHasher.HashPassword("testuser2")
                        }
                    }
                },
                new User{
                    UserId =  "6666",
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            ProviderType = LoginProvider.UserNamePassword,
                            ProviderId = "testuser3",
                            ProviderData = passwordHasher.HashPassword("testuser3")
                        }
                    }
                }
            };
            if (includeLoadTestUsers)
            {
                result.AddRange(GenerateForLoadTest(10000, passwordHasher));
            }
            return result;
        }

        private static List<User> GenerateForLoadTest(int count, IPasswordHasher passwordHasher)
        {
            return Enumerable.Range(0, count)
                .Select(i => new User
                {
                    UserId = $"subj{i}",
                    Role = RoleNames.Player,
                    IsActive = true,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            ProviderType = LoginProvider.UserNamePassword,
                            ProviderId = $"loadUser{i}",
                            ProviderData = passwordHasher.HashPassword($"loadUser{i}")
                        }
                    }
                })
                .ToList();
        }
    }
}
