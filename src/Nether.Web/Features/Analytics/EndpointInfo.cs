// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Analytics
{
    public class EndpointInfo
    {
        public string KeyName { get; set; }
        public string AccessKey { get; set; }
        public string Resource { get; set; }
        public TimeSpan Ttl { get; set; }
    }
}
