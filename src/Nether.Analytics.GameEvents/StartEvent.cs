// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics.GameEvents
{
    public class StartEvent : IGameEvent
    {
        public string Type => "start";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string EventCorrelationId { get; set; }
        public string DisplayName { get; set; }
        public string GameSessionId { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}