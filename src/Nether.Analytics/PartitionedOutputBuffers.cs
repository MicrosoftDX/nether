// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Nether.Analytics
{
    public class PartitionedOutputBuffers
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, StringBuilder>> _partitionBuffers =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, StringBuilder>>();

        public void Append(string partitionId, string bufferName, string value)
        {
            var buffersForPartition = _partitionBuffers.GetOrAdd(partitionId, (p) => new ConcurrentDictionary<string, StringBuilder>());
            var sb = buffersForPartition.GetOrAdd(bufferName, (b) => new StringBuilder());

            sb.Append(value);
        }

        public IEnumerable<KeyValuePair<string, string>> PopBuffers(string partitionId)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (_partitionBuffers.TryRemove(partitionId, out var buffersForPartition))
            {
                foreach (var buffer in buffersForPartition)
                {
                    result.Add(new KeyValuePair<string, string>(buffer.Key, buffer.Value.ToString()));
                }
            }

            return result;
        }
    }
}
