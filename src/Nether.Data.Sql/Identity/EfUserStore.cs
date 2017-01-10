// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nether.Data.Identity;
using System;
using System.Threading.Tasks;

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

        public async Task<User> GetUserByFacebookIdAsync(string facebookUserId)
        {
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.FacebookUserId == facebookUserId);
            return userEntity.Map();
        }

        public async Task<User> GetUserByIdAsync(string userid)
        {
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userid);
            return userEntity.Map();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
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
    }
}
