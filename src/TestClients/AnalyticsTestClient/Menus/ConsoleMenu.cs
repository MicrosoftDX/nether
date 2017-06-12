// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace AnalyticsTestClient
{
    public abstract class ConsoleMenu
    {
        protected string Title { get; set; }

        protected Dictionary<char, ConsoleMenuItem> MenuItems { get; } = new Dictionary<char, ConsoleMenuItem>();

        public void Show()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(Title);
                Console.WriteLine("------------------------------------------------------------");

                PrintHeader();

                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine($" {menuItem.Key} - {menuItem.Value.Text}");
                }

                Console.WriteLine("<ESC> - Back/Exit");

                PrintFooter();

                Console.WriteLine();
                Console.Write("Select an option: ");
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }

                var c = key.KeyChar;

                if (MenuItems.ContainsKey(c))
                {
                    // Execute Selected Menu Action
                    MenuItems[c].MenuAction();
                }
                else
                {
                    Console.WriteLine($"{c} is not an valid option");
                    Console.WriteLine();
                }
            }
        }

        protected virtual void PrintHeader()
        {
        }

        protected virtual void PrintFooter()
        {
        }

        public Dictionary<string, string> EditDictionary(string propertyName)
        {
            Console.WriteLine($"Fill {propertyName} with key value pairs. End with empty key.");

            var dict = new Dictionary<string, string>();
            int i = 1;

            while (true)
            {
                Console.Write($"  Key{i++}: ");
                var key = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(key))
                    break;
                else if (dict.ContainsKey(key))
                {
                    Console.WriteLine("    Can't add a key that already exists");
                    continue;
                }

                Console.Write($"  Value: ");
                var value = Console.ReadLine();

                dict.Add(key, value);
            }

            return dict;
        }

        public object EditProperty(string propertyName, object o, Type propertyType)
        {
            //TODO: ReFactor to have different methods per property type

            if (propertyType == typeof(Dictionary<string, string>))
            {
                return EditDictionary(propertyName);
            }

            while (true)
            {
                Console.Write($"{propertyName} [{o}]: ");
                var s = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(s))
                {
                    // Don't change the value of o, just return by breaking out of the loop
                    return o;
                }

                if (propertyType == typeof(string))
                {
                    return s;
                }
                else if (propertyType == typeof(int))
                {
                    int i;

                    if (int.TryParse(s, out i))
                        return i;
                }
                else if (propertyType == typeof(long))
                {
                    long l;

                    if (long.TryParse(s, out l))
                        return l;
                }
                else if (propertyType == typeof(float))
                {
                    float f;

                    if (float.TryParse(s, out f))
                        return f;
                }
                else if (propertyType == typeof(double))
                {
                    double d;

                    if (double.TryParse(s, out d))
                        return d;
                }
                else if (propertyType == typeof(DateTime))
                {
                    DateTime date;

                    if (DateTime.TryParse(s, out date))
                        return date;
                }

                Console.WriteLine($"Unable to parse {s} as {o.GetType()}");
            }
        }

        public bool EscPressed()
        {
            if (Console.KeyAvailable)
            {
                return Console.ReadKey().Key == ConsoleKey.Escape;
            }

            return false;
        }
    }
}