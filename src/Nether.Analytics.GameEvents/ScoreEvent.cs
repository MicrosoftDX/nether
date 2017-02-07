// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics.GameEvents
{
    public class ScoreEvent : IGameEvent
    {
        public string Type => "score";
        public string Version => "1.0.0";
        public DateTime ClientUtcTime { get; set; }
        public string GameSessionId { get; set; }
        public long Score { get; set; }
    }
}