// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics.MessageFormats
{
    public class SessionStart : INetherMessage
    {
        public string Type => "session-start";
        public string Version => "1.0.0";
        //public DateTime? DbgEnqueuedTimeUtc { get; set; }
        public string GameSession { get; set; }
        public string GamerTag { get; set; }
    }
}
