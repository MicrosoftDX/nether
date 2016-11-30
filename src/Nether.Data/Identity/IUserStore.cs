// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Identity
{
    public interface IUserStore
    {
        Task<User> GetUserByIdAsync(string userid);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByFacebookIdAsync(string facebookUserId);
        Task SaveUserAsync(User user);
    }
}
