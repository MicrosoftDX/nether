// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Nether.Web.Features.Identity.Models.UserAdmin
{
    public class UserRequestModel
    {
        [Required]
        public bool Active { get; set; }
        [Required]
        public string Role { get; set; }

        public static Data.Identity.User MapToUser(UserRequestModel userRequestModel, string userId)
        {
            return new Data.Identity.User
            {
                UserId = userId,
                Role = userRequestModel.Role,
                IsActive = userRequestModel.Active
            };
        }
    }
}
