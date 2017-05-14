// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.MessageFormats
{
    [Obsolete]
    public class CountMessage : INetherMessage
    {
        public string Type => "count";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string DisplayName { get; set; }
        public int Value { get; set; }
        public string GameSessionId { get; set; }
    }
}