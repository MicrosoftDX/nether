// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Text;

namespace Nether.Analytics
{
    public class CsvOutputFormatter : IOutputFormatter
    {
        public char Separator { get; set; } = ',';
        public string EmptyValue { get; set; } = "";
        public string[] Columns { get; set; }
        public bool IncludeHeaderRow { get; set; } = true;
        public string FileExtension => "csv";

        public CsvOutputFormatter(params string[] columns)
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

            builder.AppendLine();

            return builder.ToString();
        }

        public string Header
        {
            get
            {
                return string.Join(Separator.ToString(), Columns) + Environment.NewLine;
            }
        }
    }
}
