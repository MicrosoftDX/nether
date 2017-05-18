// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Nether.Analytics
{
    /// <summary>
    /// Enables reading of results from arbitrary sources.
    /// </summary>
    public interface IResultsReader
    {
        /// <summary>
        /// Retrieves the latest result set from the underlying source.
        /// </summary>
        /// <param name="max">The maximum number of results to retrieve.</param>
        /// <returns>Returns an enumrable list of <see cref="Message"/>.</returns>
        IEnumerable<Message> GetLatest(int max = 100);
    }
}
