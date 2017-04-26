// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Nether.Analytics
{
    public class Message : IKnownMessageType
    {
        public string MessageType { get; set; }
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
    }
}