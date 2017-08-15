// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Identity
{
    public static class LoginProvider
    {
        public const string UserNamePassword = "password";
        public const string FacebookUserAccessToken = "Facebook"; // use the same identifier as the implicit flow for facebook
        public const string GuestAccess = "guest-access";
    }
}
