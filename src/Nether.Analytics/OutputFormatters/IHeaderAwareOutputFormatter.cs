// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics
{
    /// <summary>
    /// The interface enables the output formatter to be aware of the header. 
    /// It also provides certain methods that enable features like serializing the 
    /// message with the header for certain scenarios (i.e. CSV).
    /// </summary>
    public interface IHeaderAwareOutputFormatter : IOutputFormatter
    {
        string Header { get; }

        string FormatWithHeaders(Message msg);
    }
}
