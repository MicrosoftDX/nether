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

        public object EditProperty<T>(string propertyDisplayName, T o)
        {
            while (true)
            {
                Console.Write($"{propertyDisplayName} [{o}]: ");
                var s = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(s))
                {
                    // Don't change the value of o, just return by breaking out of the loop
                    return o;
                }

                if (typeof(T) == typeof(string))
                {
                    return s;
                }
                else if (o is int)
                {
                    int i;

                    if (int.TryParse(s, out i))
                        return i;
                }
                else if (o is long)
                {
                    long l;

                    if (long.TryParse(s, out l))
                        return l;
                }
                else if (o is float)
                {
                    float f;

                    if (float.TryParse(s, out f))
                        return f;
                }
                else if (o is double)
                {
                    double d;

                    if (double.TryParse(s, out d))
                        return d;
                }
                else if (o is DateTime)
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