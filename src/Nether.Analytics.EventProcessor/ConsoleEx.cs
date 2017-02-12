// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics.EventProcessor
{
    public static class ConsoleEx
    {
        public static void WriteConnectionString(string connectionString, int indent = 0)
        {
            var lines = connectionString.Split(';');

            foreach (var line in lines)
            {
                Console.WriteLine("".PadLeft(indent) + MaskKey(line));
            }
        }

        private static string MaskKey(string line)
        {
            const string sharedAccessKey = "SharedAccessKey=";

            var compressedLine = line.Replace(" ", "");

            if (!compressedLine.StartsWith(sharedAccessKey)) return compressedLine;

            var key = compressedLine.Substring(sharedAccessKey.Length);
            var maskedKey = $"{key.Substring(0, 5)}.....{key.Substring(key.Length - 5)}";
            return sharedAccessKey + maskedKey;
        }
    }
}