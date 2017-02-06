using System;
using AnalyticsTestClient.Utils;

namespace AnalyticsTestClient
{
    public class SetupMenu : ConsoleMenu
    {
        public SetupMenu()
        {
            Title = "Nether Analytics Test Client - Setup Menu";

            MenuItems.Add('1', new ConsoleMenuItem($"Set Event Hub Connection String", SetEventHubConnectionString));
            MenuItems.Add('2', new ConsoleMenuItem($"Set Event Hub Name/Path", SetEventHubNamePath));
        }

        protected override void PrintFooter()
        {
            ShowCurrentConfig();
        }

        private void SetEventHubConnectionString()
        {
            ConfigCache.EventHubConnectionString = (string)EditProperty("Event Hub Connection String", ConfigCache.EventHubConnectionString);
        }

        private void SetEventHubNamePath()
        {
            ConfigCache.EventHubName = (string)EditProperty("Event Hub Name", ConfigCache.EventHubName);
        }

        public static void ShowCurrentConfig()
        {
            Console.WriteLine();
            Console.WriteLine("Using:");
            Console.WriteLine($"  Event Hub Connection String:");
            PrintConnectionString(ConfigCache.EventHubConnectionString, 4);
            Console.WriteLine($"  Event Hub Name/Path: {ConfigCache.EventHubName??"not specified"}");
            Console.WriteLine();
        }

        private static void PrintConnectionString(string connectionString, int indent=0)
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