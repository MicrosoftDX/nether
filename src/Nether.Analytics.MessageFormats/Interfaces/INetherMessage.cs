// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.MessageFormats
{
    public interface INetherMessage
    {
        string Type { get; }
        string Version { get; }
        DateTime ClientUtcTime { get; set; }
        //Dictionary<string, string> Properties { get; }
    }
}