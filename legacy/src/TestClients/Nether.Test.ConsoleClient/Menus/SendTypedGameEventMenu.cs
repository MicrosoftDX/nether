// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Ingest;
using Nether.Ingest.MessageFormats;
using System;
using System.Linq;
using System.Reflection;

namespace Nether.Test.ConsoleClient
{
    public class SendTypedGameEventMenu : ConsoleMenu
    {
        private static Type[] s_messageTypes = null;
        private IAnalyticsClient _client;

        public SendTypedGameEventMenu(IAnalyticsClient client)
        {
            _client = client;

            Title = "Nether Analytics Test Client - Send Static Game Messages";

            var msgTypes = GetMessageTypes();
            var menuKey = 'a';
            foreach (var msgType in msgTypes)
            {
                var msg = (ITypeVersionMessage)Activator.CreateInstance(msgType);
                MenuItems.Add(menuKey++,
                    new ConsoleMenuItem(
                        $"{msg.Type}, {msg.Version}",
                        () => TypedMessageSelected(msg)));
            }

            MenuItems.Add('1', new ConsoleMenuItem("Loop and send random messages", LoopAndSendRandom));
            MenuItems.Add('2', new ConsoleMenuItem("Send using sample messages as template", () => { new SendSampleMessageMenu(_client).Show(); }));
        }

        private void LoopAndSendRandom()
        {
            var j = ConsoleEx.ReadLine("Number of messages to send (0 for infinite or until ESC is pressed)", 0);

            for (var i = 0; i < j || j == 0; i++)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }

                var typeToSend = GetMessageTypes().TakeRandom();
                var gameEvent = (ITypeVersionMessage)Activator.CreateInstance(typeToSend);

                var props = gameEvent.GetType().GetProperties();

                foreach (var prop in props)
                {
                    var propName = prop.Name;
                    if (propName != "ClientUtcTime" && Program.PropertyCache.ContainsKey(propName))
                    {
                        prop.SetValue(gameEvent, Program.PropertyCache[propName]);
                    }
                }

                _client.SendMessageAsync(gameEvent).Wait();
            }

            _client.FlushAsync().Wait();
        }

        private static Type[] GetMessageTypes()
        {
            if (s_messageTypes == null)
            {
                var assembly = typeof(HeartBeat).GetTypeInfo().Assembly;
                var types = assembly.GetTypes();
                var gameEventTypes =
                    from t in types
                    where typeof(ITypeVersionMessage).IsAssignableFrom(t) && t != typeof(ITypeVersionMessage)
                    orderby t.Name
                    select t;

                s_messageTypes = gameEventTypes.ToArray();
            }

            return s_messageTypes;
        }

        private void TypedMessageSelected(ITypeVersionMessage gameEvent)
        {
            EditMessageProperties(gameEvent);
            _client.SendMessageAsync(gameEvent).Wait();
            _client.FlushAsync().Wait();
        }

        private void EditMessageProperties(ITypeVersionMessage gameEvent)
        {
            var props = gameEvent.GetType().GetProperties();

            foreach (var prop in props)
            {
                var propName = prop.Name;
                if (propName != "ClientUtcTime" && Program.PropertyCache.ContainsKey(propName))
                {
                    prop.SetValue(gameEvent, Program.PropertyCache[propName]);
                }
                var propValue = prop.GetValue(gameEvent);

                if (propName == "Type" || propName == "Version")
                {
                    // Don't ask for input for these properties, just display their
                    // values since they are static and can't be changed
                    Console.WriteLine($"{propName}: {propValue}");
                }
                else
                {
                    var propertyType = prop.PropertyType;
                    var o = EditProperty(propName, propValue, propertyType);
                    prop.SetValue(gameEvent, o);
                    Program.PropertyCache[propName] = o;
                }
            }
        }
    }
}