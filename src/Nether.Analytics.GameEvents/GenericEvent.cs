// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.GameEvents
{
    public class GenericEvent : IGameEvent
    {
        public string Type => "generic";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}