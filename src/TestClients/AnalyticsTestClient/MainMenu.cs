// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using AnalyticsTestClient.Utils;

namespace AnalyticsTestClient
{
    public class MainMenu : ConsoleMenu
    {
        public MainMenu()
        {
            Title = "Nether Analytics Test Client - Main Menu";

            MenuItems.Add('1', new ConsoleMenuItem("Setup ...", () => { new SetupMenu().Show(); }));
            MenuItems.Add('2', new ConsoleMenuItem("Send Typed Game Events ...", () => { new SendTypedGameEventMenu().Show(); }));
            MenuItems.Add('3', new ConsoleMenuItem("Send Custom Game Event", SendCustomGameEvent));
            MenuItems.Add('4', new ConsoleMenuItem("Re-send Last Sent Message", ResendLastSentMessage));
        }

        private void SendCustomGameEvent()
        {
            var msg = (string)EditProperty("Custom Message", $"This is a custom msg at {DateTime.UtcNow}", typeof(string));

            EventHubManager.SendMessageToEventHub(msg).Wait();
        }

        private void ResendLastSentMessage()
        {
            EventHubManager.ReSendLastMessageToEventHub().Wait();
        }
    }
}