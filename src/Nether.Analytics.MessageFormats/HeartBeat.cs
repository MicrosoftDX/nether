// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics.MessageFormats
{
    public class HeartBeat : ITypeVersionMessage
    {
        public string Type => "heartbeat";
        public string Version => "1.0.0";
        public string GameSession { get; set; }
    }
}
