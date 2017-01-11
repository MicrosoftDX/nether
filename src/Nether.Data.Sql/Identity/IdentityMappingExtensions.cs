// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Sql.Identity
{
    public static class IdentityMappingExtensions
    {
        public static User Map(this UserEntity entity)
        {
            if (entity == null)
                return null;

            var user = new User();
            entity.MapTo(user);
            return user;
        }
        public static void MapTo(this UserEntity entity, User user)
        {
            if (entity != null)
            {
                user.UserId = entity.UserId;
                user.UserName = entity.UserName;
                user.IsActive = entity.IsActive;
                user.FacebookUserId = entity.FacebookUserId;
                user.Role = entity.Role;
                user.PasswordHash = entity.PasswordHash;
            }
        }
        public static UserEntity Map(this User user)
        {
            return user == null
                ? null
                : new UserEntity
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    IsActive = user.IsActive,
                    FacebookUserId = user.FacebookUserId,
                    Role = user.Role,
                    PasswordHash = user.PasswordHash
                };
        }
    }
}
