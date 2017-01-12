// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Data.Identity
{
    public class Login
    {
        public string ProviderType { get; set; }
        public string ProviderId { get; set; }
        public string ProviderData { get; set; }
    }
}
