// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Web.Features.Identity
{
    public class SignInMethodOptions
    {
        public FacebookOptions Facebook { get; set; }
        public GuestAccessOptions GuestAccess { get; set; }
        public UsernamePasswordOptions UsernamePassword { get; set; }
    }

    public class FacebookOptions
    {
        public bool EnableImplicit { get; set; }
        public bool EnableAccessToken { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
    public class GuestAccessOptions
    {
        public bool Enabled { get; set; }
    }
    public class UsernamePasswordOptions
    {
        public bool AllowUserSignUp { get; set; }
    }
}