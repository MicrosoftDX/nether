// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using IdentityModel;
using IdentityServer4.Services.InMemory;
using Nether.Data.Identity;
using Nether.Web.Features.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity
{
    // TODO - remove this class (or move to Data.InMemory if we decide to keep it!!)
    // Also remove Users class under Configuration

    public class InMemoryUserStore : IUserStore
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly List<InMemoryUser> _users;
        public InMemoryUserStore(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _users = Users.Get();
        }
        private User Map(InMemoryUser inMemoryUser)
        {
            if (inMemoryUser == null)
            {
                return null;
            }
            return new User
            {
                UserId = inMemoryUser.Subject,
                UserName = inMemoryUser.Username,
                IsActive = inMemoryUser.Enabled,
                Role = inMemoryUser.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role)?.Value,
                PasswordHash = inMemoryUser.Password == null ? null : _passwordHasher.HashPassword(inMemoryUser.Password), // nice lazy way to not change the inmemory users :-)
                FacebookUserId = inMemoryUser.ProviderId // assuming Provider == facebook ;-)
            };
        }
        private InMemoryUser Map(User user)
        {
            return new InMemoryUser
            {
                Subject = user.UserId,
                Username = user.UserName,
                Enabled = user.IsActive,
                // skipping password/password hash for now
                ProviderId = user.FacebookUserId,
                Claims = new Claim[]
                {
                    new Claim(JwtClaimTypes.Role, user.Role),
                }
            };
        }
        public Task<User> GetUserByIdAsync(string userid)
        {
            var query =
                from u in _users
                where u.Subject == userid
                select u;
            var user = query.SingleOrDefault();
            return Task.FromResult(Map(user));
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            var query =
                from u in _users
                where u.Username == username
                select u;
            var user = query.SingleOrDefault();
            return Task.FromResult(Map(user));
        }

        public Task<User> GetUserByFacebookIdAsync(string facebookUserId)
        {
            var query =
                from u in _users
                where u.ProviderId == facebookUserId
                select u;
            var user = query.SingleOrDefault();
            return Task.FromResult(Map(user));
        }

        public Task SaveUserAsync(User user)
        {
            var inMemoryUser = Map(user);
            _users.RemoveAll(u => u.Subject == inMemoryUser.Subject);
            _users.Add(inMemoryUser);
            return Task.CompletedTask;
        }
    }
}
