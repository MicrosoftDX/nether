// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Nether.Data.Sql.Identity
{
    public class EntityFrameworkUserStore : IUserStore
    {
        private IdentityContext _context;

        private readonly ILogger _logger;

        public EntityFrameworkUserStore(IdentityContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<EntityFrameworkUserStore>();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var userEntities = await _context.Users.ToListAsync();
            return userEntities
                    .Select(u => u.Map());
        }

        public async Task<User> GetUserByIdAsync(string userid)
        {
            var userEntity = await _context.Users
                .Include(u => u.Logins)
                .FirstOrDefaultAsync(u => u.UserId == userid);
            return userEntity.Map();
        }

        public async Task<User> GetUserByLoginAsync(string providerType, string providerId)
        {
            var userEntity = await _context.Users
                .Include(u => u.Logins)
                .FirstOrDefaultAsync(u =>
                        u.Logins.Any(l => l.ProviderType == providerType && l.ProviderId == providerId));
            return userEntity.Map();
        }

        public async Task SaveUserAsync(User user)
        {
            var userEntity = user.Map();
            // assume that the user is new if UserId is null, and existing if it is set
            if (userEntity.UserId == null)
            {
                userEntity.UserId = Guid.NewGuid().ToString("d");
                _context.Add(userEntity);
            }
            else
            {
                _context.Update(userEntity);
            }
            await _context.SaveChangesAsync();
            // update user with any changes from saving
            userEntity.MapTo(user);
        }

        public async Task DeleteUserAsync(string userId)
        {
            _context.Users.Remove(new UserEntity { UserId = userId });
            await _context.SaveChangesAsync();
        }
    }
}
