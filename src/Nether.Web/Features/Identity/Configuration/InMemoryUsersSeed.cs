// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        public static List<Data.Identity.User> Get(IPasswordHasher passwordHasher)
        {
            var result = new List<Data.Identity.User>
            {
                new Data.Identity.User {
                    UserId = "1111",
                    UserName =  "devuser",
                    PasswordHash = passwordHasher.HashPassword("devuser"),
                    Role = RoleNames.Player,
                    IsActive = true
                },
                new Data.Identity.User {
                    UserId = "2222",
                    UserName =  "devadmin",
                    PasswordHash = passwordHasher.HashPassword("devadmin"),
                    Role = RoleNames.Admin,
                    IsActive = true
                },
                // Integration test users
                // user without gamertag
                new Data.Identity.User{
                    UserId =  "112299", UserName =   "testuser-notag",
                    PasswordHash = passwordHasher.HashPassword("password123"),
                    Role = RoleNames.Player,
                    IsActive = true
                },
                // user with gamertag
                new Data.Identity.User {
                    UserId =  "3333",
                    UserName =  "testuser",
                    PasswordHash = passwordHasher.HashPassword("testuser"),
                    Role = RoleNames.Player,
                    IsActive = true
                },
                new Data.Identity.User{
                    UserId =  "4444",
                    UserName =  "testuser1",
                    PasswordHash = passwordHasher.HashPassword("testuser1"),
                    Role = RoleNames.Player,
                    IsActive = true
                },
                new Data.Identity.User{
                    UserId =  "5555",
                    UserName =  "testuser2",
                    PasswordHash = passwordHasher.HashPassword("testuser2"),
                    Role = RoleNames.Player,
                    IsActive = true
                },
                new Data.Identity.User{
                    UserId =  "6666",
                    UserName =  "testuser3",
                    PasswordHash = passwordHasher.HashPassword("testuser3"),
                    Role = RoleNames.Player,
                    IsActive = true
                }
            };
            result.AddRange(GenerateForLoadTest(10000, passwordHasher));
            return result;
        }

        private static List<Data.Identity.User> GenerateForLoadTest(int count, IPasswordHasher passwordHasher)
        {
            return Enumerable.Range(0, count)
                .Select(i => new Data.Identity.User
                {
                    UserId = $"subj{i}",
                    UserName = $"loadUser{i}",
                    PasswordHash = passwordHasher.HashPassword($"loadUser{i}"),
                    Role = RoleNames.Player,
                    IsActive = true
                })
                .ToList();
        }
    }
}
