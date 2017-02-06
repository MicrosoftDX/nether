using System;
using System.Collections.Generic;

namespace AnalyticsTestClient
{
    public class Menu
    {
        private readonly string _title;
        private readonly Dictionary<char, MenuItem> _menuItems = new Dictionary<char, MenuItem>();

        public Menu(string title, params MenuItem[] menuItems)
        {
            _title = title;
            foreach (var menuItem in menuItems)
            {
                _menuItems.Add(menuItem.Key, menuItem);
            }
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(_title);
                Console.WriteLine("------------------------------------------------------------");
                foreach (var menuItem in _menuItems.Values)
                {
                    Console.WriteLine($" {menuItem.Key} - {menuItem.Text}");
                }
                Console.WriteLine();
                Console.Write("Select an option: ");
                var key = Console.ReadKey();
                Console.WriteLine();

                char c;

                if (key.Key == ConsoleKey.Enter ||
                    key.Key == ConsoleKey.Escape ||
                    key.Key == ConsoleKey.Backspace)
                {
                    c = '0';
                }
                else
                {
                    c = key.KeyChar;
                }
                
                if (_menuItems.ContainsKey(c))
                {
                    // Execute Selected Menu Action
                    _menuItems[c].MenuAction();
                    break;
                }

                Console.WriteLine($"{c} is not an valid option");
                Console.WriteLine();
            }
        }
    }
}