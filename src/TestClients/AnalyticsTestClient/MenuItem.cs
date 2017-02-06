using System;

namespace AnalyticsTestClient
{
    public class MenuItem
    {
        public MenuItem(char key, string text, Action menuAction)
        {
            Key = key;
            Text = text;
            MenuAction = menuAction;
        }

        public char Key { get; }

        public string Text { get; }

        public Action MenuAction { get; }
    }
}