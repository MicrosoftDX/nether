// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.EntityFramework.Identity
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

            var entity = new UserEntity();
            user.MapTo(entity);
            return entity;
        }
        public static UserEntity MapTo(this User user, UserEntity userEntity)
        {
            userEntity.UserId = user.UserId;
            userEntity.IsActive = user.IsActive;
            userEntity.Role = user.Role;
            if (userEntity.Logins == null)
            {
                userEntity.Logins = new List<LoginEntity>();
            }

            // merge the login values to ensure correct change tracking with EF
            if (user.Logins == null)
            {
                userEntity?.Logins?.Clear();
            }
            else
            {
                foreach (var login in user.Logins)
                {
                    var loginEntity = userEntity.Logins
                                        .FirstOrDefault(l => l.ProviderType == login.ProviderType && l.ProviderId == login.ProviderId);
                    if (loginEntity == null)
                    {
                        loginEntity = new LoginEntity
                        {
                            User = userEntity,
                            UserId = userEntity.UserId
                        };
                        userEntity.Logins.Add(loginEntity);
                    }
                    loginEntity.ProviderType = login.ProviderType;
                    loginEntity.ProviderId = login.ProviderId;
                    loginEntity.ProviderData = login.ProviderData;
                }
                var loginsToRemove = userEntity?.Logins
                                        ?.Where(l => !user.Logins.Any(l2 => l.ProviderType == l2.ProviderType && l.ProviderId == l2.ProviderId));
                foreach (var loginToRemove in loginsToRemove)
                {
                    userEntity.Logins.Remove(loginToRemove);
                }
            }

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
