// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace Nether.Analytics.EventProcessor
{
    public static class StringBuilderExtensions
    {
        private const string ColumnDelimiter = "|";
        private const string PropertyDelimiter = ";";

        public static void AppendColumns(this StringBuilder builder, params object[] values)
        {
            AppendSeparatedValues(builder, ColumnDelimiter, values);
        }

        public static void AppendProperties(this StringBuilder builder, params object[] values)
        {
            AppendSeparatedValues(builder, PropertyDelimiter, values);
        }

        private static void AppendSeparatedValues(this StringBuilder builder, string delimiter, params object[] values)
        {
            foreach (var val in values)
            {
                if (builder.Length > 0)
                    builder.Append(delimiter);

                builder.Append(val);
            }
        }
    }
}