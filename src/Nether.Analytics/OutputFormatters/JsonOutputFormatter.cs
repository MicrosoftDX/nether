// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Nether.Analytics
{
    public class JsonOutputFormatter : IOutputFormatter
    {
        public string FileExtension => "json";

        public string Header => throw new NotSupportedException("Json does not support outputting headers.");

        public bool IncludeHeaders { get; set; } = true;

        public string Format(Message msg)
        {
            return JsonConvert.SerializeObject(msg.Properties) + Environment.NewLine;
        }
    }
}
