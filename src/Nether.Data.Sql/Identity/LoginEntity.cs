// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Nether.Data.Sql.Identity
{
    [DebuggerDisplay("LoginEntity (UserId '{UserId}', Type '{ProviderType}', Id '{ProviderId}')")]
    public class LoginEntity
    {
        public string ProviderType { get; set; }
        public string ProviderId { get; set; }

        public string UserId { get; set; }
        public UserEntity User { get; set; }

        public string ProviderData { get; set; }
    }
}