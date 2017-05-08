// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Text;

namespace Nether.Analytics
{
    public class MessageCsvSerializer : IMessageSerializer
    {
        private readonly MessageCsvSerializerOptions _options;

        public MessageCsvSerializer(params string[] columns) : this(new MessageCsvSerializerOptions { Columns = columns })
        {
        }

        public MessageCsvSerializer(MessageCsvSerializerOptions options = null)
        {
            _options = options ?? new MessageCsvSerializerOptions();
        }

        public string FileExtension => "csv";

        public string Serialize(Message message)
        {
            string[] columns;

            if (_options.Columns?.Length > 0)
            {
                // Use specified columns
                columns = _options.Columns;
            }
            else
            {
                // If no columns are specified, then use all keys in dictionary as colums
                columns = message.Properties.Keys.ToArray();
            }

            var builder = new StringBuilder();

            if (_options.IncludeHeaderRow)
            {
                foreach (var column in columns)
                {
                    builder.Append(column);
                    builder.Append(_options.Separator);
                }
                builder.AppendLine();
            }

            foreach (var column in columns)
            {
                if (message.Properties.TryGetValue(column, out var value))
                {
                    builder.Append(value);
                }
                else
                {
                    builder.Append(_options.EmptyValue);
                }

                builder.Append(_options.Separator);
            }

            return builder.ToString();
        }
    }
}
