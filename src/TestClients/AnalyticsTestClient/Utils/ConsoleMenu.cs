// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace AnalyticsTestClient.Utils
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

        public object EditProperty(string propertyName, object o, Type propertyType)
        {
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
    }
}