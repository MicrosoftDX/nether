// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Models
{
    public class UserRequestModel
    {
        [Required]
        public bool Active { get; set; }
        [Required]
        public string Role { get; set; }

        public static User MapToUser(UserRequestModel userRequestModel, string userId)
        {
            return new User
            {
                UserId = userId,
                Role = userRequestModel.Role,
                IsActive = userRequestModel.Active
            };
        }
    }
}
