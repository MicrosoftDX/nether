// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics
{
    /// <summary>
    /// Created in order to test .Now related tests by modifying "system" time
    /// </summary>
    public interface ITimeProvider
    {
        DateTime GetUtcNow();
    }

    /// <summary>
    /// Returns current UTC Time
    /// </summary>
    public class SystemTimeProvider : ITimeProvider
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
