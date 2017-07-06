// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Ingest.MessageFormats
{
    public class SessionStart : ITypeVersionMessage
    {
        public string Type => "session-start";
        public string Version => "1.0.0";
        public string GameSession { get; set; }
        public string GamerTag { get; set; }
    }
}
