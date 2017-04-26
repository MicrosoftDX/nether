// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Analytics
{
    public class MessageCsvSerializerOptions
    {
        private readonly char _separator = '\t';
        private readonly bool _includeHeaderRow = false;
        private readonly string[] _columns;

        public MessageCsvSerializerOptions()
        {
        }

        public MessageCsvSerializerOptions(params string[] columns)
        {
            _columns = columns;
        }

        public MessageCsvSerializerOptions(char separator, bool includeHeaderRow = false, params string[] columns)
        {
            _separator = separator;
            _includeHeaderRow = includeHeaderRow;
            _columns = columns;
        }

        public char Separator => _separator;
        public bool IncludeHeaderRow => _includeHeaderRow;
        public string[] Columns => _columns;
    }
}
