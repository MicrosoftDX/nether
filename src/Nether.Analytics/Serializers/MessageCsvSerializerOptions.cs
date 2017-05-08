// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics
{
    public class MessageCsvSerializerOptions
    {
        public char Separator { get; set; } = '\t';
        public string EmptyValue { get; set; } = "";
        public bool IncludeHeaderRow { get; set; } = false;
        public string[] Columns { get; set; }

    }
}
