using System;

namespace AnalyticsTestClient.Utils
{
    public class ConsoleMenuItem
    {
        public ConsoleMenuItem(string text, Action menuAction)
        {
            Text = text;
            MenuAction = menuAction;
        }

        public string Text { get; }

        public Action MenuAction { get; }
    }
}