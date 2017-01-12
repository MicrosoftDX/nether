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
                user.IsActive = entity.IsActive;
                user.Role = entity.Role;

                // currently recreating the Login collection for simplicity
                user.Logins = new List<Login>();
                if (entity.Logins != null)
                {
                    foreach (var loginEntity in entity.Logins)
                    {
                        user.Logins.Add(loginEntity.Map());
                    }
                }
            }
        }
        public static Login Map(this LoginEntity entity)
        {
            if (entity == null)
                return null;

            return new Login
            {
                ProviderType = entity.ProviderType,
                ProviderId = entity.ProviderId,
                ProviderData = entity.ProviderData
            };
        }
        public static UserEntity Map(this User user)
        {
            if (user == null)
                return null;
            var userEntity = new UserEntity
            {
                UserId = user.UserId,
                IsActive = user.IsActive,
                Role = user.Role,
            };
            userEntity.Logins = user.Logins.Select(l => l.Map(userEntity)).ToList();
            return userEntity;
        }
        public static LoginEntity Map(this Login login, UserEntity userEntity)
        {
            return new LoginEntity
            {
                User = userEntity,
                UserId = userEntity.UserId,
                ProviderType = login.ProviderType,
                ProviderId = login.ProviderId,
                ProviderData = login.ProviderData
            };
        }
    }
}
