// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics.MessageFormats
{
    public interface INetherMessage
    {
        string Type { get; }
        string Version { get; }
        //DateTime? DbgEnqueuedTimeUtc { get; }
    }
}