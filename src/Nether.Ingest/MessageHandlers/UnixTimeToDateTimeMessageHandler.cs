// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Ingest
{
    public class UnixTimeToDateTimeMessageHandler : IMessageHandler
    {
        private HashSet<string> _propertiesToChange;

        public UnixTimeToDateTimeMessageHandler(params string[] properties)
        {
            _propertiesToChange = new HashSet<string>(properties);
        }

        public Task<MessageHandlerResults> ProcessMessageAsync(Message msg, string pipelineName, int index)
        {
            foreach (var key in msg.Properties.Keys.ToArray())
            {
                if (_propertiesToChange.Contains(key))
                {
                    long seconds = long.Parse(msg.Properties[key]);
                    var offset = DateTimeOffset.FromUnixTimeSeconds(seconds);

                    msg.Properties[key] = offset.UtcDateTime.ToString();
                }
            }

            return Task.FromResult(MessageHandlerResults.Success);
        }
    }
}