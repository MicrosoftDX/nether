// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics.Host
{
    public static class ConsoleEx
    {
        public static void Write(ConsoleColor color, string value)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ForegroundColor = currentColor;
        }

        public static void WriteLine(ConsoleColor color, string value)
        {
            Write(color, value + Environment.NewLine);
        }
    }
}
