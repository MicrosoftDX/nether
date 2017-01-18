// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Models
{
    public class UserResponseModel
    {
        public UserModel User { get; set; }

        public static UserResponseModel MapFrom(User user)
        {
            return new UserResponseModel
            {
                User = new UserModel
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    Active = user.IsActive,
                    Logins = user.Logins.Select(l => new UserLoginModel
                    {
                        ProviderType = l.ProviderType,
                        ProviderId = l.ProviderId,
                        ProviderData = l.ProviderData
                    })
                    .ToList()
                }
            };
        }
        public class UserModel
        {
            public bool Active { get; set; }
            public string Role { get; set; }
            public string UserId { get; set; }
            public List<UserLoginModel> Logins { get; set; }
        }
        public class UserLoginModel
        {
            public string ProviderData { get; set; }
            public string ProviderId { get; set; }
            public string ProviderType { get; set; }
        }
    }
}
