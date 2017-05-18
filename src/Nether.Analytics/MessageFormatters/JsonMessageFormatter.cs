// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Nether.Analytics
{
    public class JsonMessageFormatter : IMessageFormatter
    {
        public string FileExtension => "json";

        public string Header { get { throw new NotSupportedException("Json does not support outputting headers."); } }

        public bool IncludeHeaders => false;

        public string Format(Message msg)
        {
            return JsonConvert.SerializeObject(msg.Properties);
        }

        public Message Parse(string input)
        {
            throw new NotImplementedException();
        }
    }
}
