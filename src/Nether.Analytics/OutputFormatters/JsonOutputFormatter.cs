// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Nether.Analytics
{
    public class JsonOutputFormatter : IOutputFormatter
    {
        public string FileExtension => "json";
        public bool IncludeHeaderRow => false;

        public string Header
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Format(Message msg)
        {
            return JsonConvert.SerializeObject(msg.Properties) + Environment.NewLine;
        }
    }
}
