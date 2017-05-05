// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Nether.Analytics
{
    public class MessageJsonSerializer : IMessageSerializer
    {
        public string Serialize(IMessage msg)
        {
            return JsonConvert.SerializeObject(msg.Properties);
        }
    }
}
