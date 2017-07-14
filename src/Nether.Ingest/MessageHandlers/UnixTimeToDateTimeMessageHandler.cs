// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
            foreach (var msgProp in msg.Properties)
            {
                if (_propertiesToChange.Contains(msgProp.Key))
                {
                    long seconds = long.Parse(msgProp.Value);
                    var offset = DateTimeOffset.FromUnixTimeSeconds(seconds);

                    msg.Properties[msgProp.Key] = offset.UtcDateTime.ToString();
                }
            }

            return Task.FromResult(MessageHandlerResults.Success);
        }
    }
}