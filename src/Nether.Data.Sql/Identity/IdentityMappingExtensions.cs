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
            return entity == null
                ? null
                : new User
                {
                    UserId = entity.UserId,
                    UserName = entity.UserName,
                    IsActive = entity.IsActive,
                    FacebookUserId = entity.FacebookUserId,
                    Role = entity.Role,
                    PasswordHash = entity.PasswordHash
                };
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
