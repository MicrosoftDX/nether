// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetherSDK.Models
{
    [Serializable]
    public class Endpoint
    {
        public string httpVerb;
        public string url;
        public string contentType;
        public string authorization;
        public string validUntilUtc;
    }
}
