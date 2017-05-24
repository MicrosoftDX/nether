// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Nether.Analytics
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

        public static void WriteConnectionString(string connectionString, int indent = 0)
        {
            var lines = (connectionString ?? string.Empty).Split(';');

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

        public static string ReadLine(string description, string defaultValue = "")
        {
            Console.Write($"{description} [{defaultValue}] : ");
            var answer = Console.ReadLine();
            answer = answer.Trim();

            return string.IsNullOrEmpty(answer) ? defaultValue : answer;
        }

        public static string ReadLine(string description, string defaultValue, Func<string, bool> checkFunc, string errorMessage = "Parameter isn't valid")
        {
            while (true)
            {
                try
                {
                    Console.Write($"{description} [{defaultValue}] : ");
                    var answer = Console.ReadLine();
                    var returnValue = string.IsNullOrEmpty(answer) ? defaultValue : answer;

                    if (checkFunc(returnValue))
                        return returnValue;
                    else
                        Console.WriteLine(errorMessage);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unknown error while checking parameter, please try again");
                }
            }
        }

        public static DateTime ReadLine(string description, DateTime defaultValue)
        {
            while (true)
            {
                try
                {
                    Console.Write($"{description} [{defaultValue}] : ");
                    var answer = Console.ReadLine();

                    return string.IsNullOrEmpty(answer) ? defaultValue : DateTime.Parse(answer);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to parse answer as datetime, please try again");
                }
            }
        }

        public static float ReadLine(string description, float defaultValue)
        {
            while (true)
            {
                try
                {
                    Console.Write($"{description} [{defaultValue}] : ");
                    var answer = Console.ReadLine();

                    return string.IsNullOrEmpty(answer) ? defaultValue : float.Parse(answer);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to parse answer as float, please try again");
                }
            }
        }

        public static double ReadLine(string description, double defaultValue)
        {
            while (true)
            {
                try
                {
                    Console.Write($"{description} [{defaultValue}] : ");
                    var answer = Console.ReadLine();

                    return string.IsNullOrEmpty(answer) ? defaultValue : double.Parse(answer);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to parse answer as double, please try again");
                }
            }
        }

        public static int ReadLine(string description, int defaultValue)
        {
            while (true)
            {
                try
                {
                    Console.Write($"{description} [{defaultValue}] : ");
                    var answer = Console.ReadLine();

                    return string.IsNullOrEmpty(answer) ? defaultValue : int.Parse(answer);
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to parse answer as int, please try again");
                }
            }
        }
    }
}