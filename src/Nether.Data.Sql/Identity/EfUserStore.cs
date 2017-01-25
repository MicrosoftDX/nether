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

        public EntityFrameworkUserStore(IdentityContext context, ILogger<EntityFrameworkUserStore> logger)
        {
            _context = context;
            _logger = logger;
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
            UserEntity userEntity;
            // assume that the user is new if UserId is null, and existing if it is set
            if (user.UserId != null)
            {
                userEntity = await _context.Users
                                        .Include(u => u.Logins) // include logins for proper merging!
                                        .FirstOrDefaultAsync(u => u.UserId == user.UserId);
                if (userEntity != null)
                {
                    user.MapTo(userEntity);
                    await _context.SaveChangesAsync();
                    // update user with any changes from saving
                    userEntity.MapTo(user);
                    return;
                }
            }

            // no user id, or we didn't find the user, so add them
            userEntity = user.Map();
            userEntity.UserId = userEntity.UserId ?? Guid.NewGuid().ToString("d");
            _context.Add(userEntity);
            await _context.SaveChangesAsync();
            // update user with any changes from saving
            userEntity.MapTo(user);
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
