// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Text;

namespace Nether.Analytics
{
    public class CsvMessageFormatter : IMessageFormatter
    {
        public char Separator { get; set; } = ',';
        public string EmptyValue { get; set; } = "";
        public string[] Columns { get; set; }

        public string FileExtension => "csv";

        public bool IncludeHeaders { get; set; } = true;

        public CsvMessageFormatter(params string[] columns)
        {
            Columns = columns;
        }

        public string Format(Message message)
        {
            string[] columns;

            if (Columns?.Length > 0)
            {
                // Use specified columns
                columns = Columns;
            }
            else
            {
                // If no columns are specified, then use all keys in dictionary as colums
                columns = message.Properties.Keys.ToArray();
            }

            var builder = new StringBuilder();

            foreach (var column in columns)
            {
                if (builder.Length > 0)
                {
                    // Append column separator if this isn't the first column
                    builder.Append(Separator);
                }

                if (message.Properties.TryGetValue(column, out var value))
                {
                    builder.Append(value);
                }
                else
                {
                    builder.Append(EmptyValue);
                }
            }

            return builder.ToString();
        }

        public Message Parse(string input)
        {
            // TODO: we should probably use a 3rd party parser! But for a POC, this will do.
            var split = input.Split(Separator);
            if (split.Length != this.Columns.Length)
            {
                throw new MissingFieldException($"The number of fields in the CSV ({split.Length}) does not correspond to the columns passed to the formatter ({this.Columns.Length}).");
            }

            var m = new Message();
            for (int i = 0; i < Columns.Length; i++)
            {
                m.Properties.Add(Columns[i], split[i]);
            }

            return m;
        }

        public string Header
        {
            get
            {
                return string.Join(Separator.ToString(), Columns);
            }
        }
    }
}