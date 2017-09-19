// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Nether.Web.Features.Identity.Models.UserAdmin
{
    public class UserResponseModel
    {
        public UserModel User { get; set; }

        public static UserResponseModel MapFrom(Data.Identity.User user, IUrlHelper url)
        {
            return new UserResponseModel
            {
                User = new UserModel
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    Active = user.IsActive,
                    Logins = user.Logins == null
                            ? new List<UserLoginModel>()
                            : user.Logins.Select(l => new UserLoginModel
                            {
                                ProviderType = l.ProviderType,
                                ProviderId = l.ProviderId,
                                _Link = url.RouteUrl(nameof(UserLoginAdminController.DeleteUserLogin), new { userId = user.UserId, providerType = l.ProviderType }, null)
                            }).ToList()
                }
            };
        }
    }
}
