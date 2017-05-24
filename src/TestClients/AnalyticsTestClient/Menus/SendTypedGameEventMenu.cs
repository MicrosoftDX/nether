// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Nether.Analytics.MessageFormats;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading;

namespace AnalyticsTestClient
{
    public class SendTypedGameEventMenu : ConsoleMenu
    {
        private static Type[] s_messageTypes = null;

        public SendTypedGameEventMenu()
        {
            Title = "Nether Analytics Test Client - Send Static Game Events";

            var msgTypes = GetMessageTypes();
            var menuKey = 'a';
            foreach (var msgType in msgTypes)
            {
                var msg = (INetherMessage)Activator.CreateInstance(msgType);
                MenuItems.Add(menuKey++,
                    new ConsoleMenuItem(
                        $"{msg.Type}, {msg.Version}",
                        () => TypedMessageSelected(msg)));
            }

            MenuItems.Add('1', new ConsoleMenuItem("Loop and send random events", LoopAndSendRandom));
        }

        private void LoopAndSendRandom()
        {
            while (true)
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
                var gameEvent = (INetherMessage)Activator.CreateInstance(typeToSend);

                var props = gameEvent.GetType().GetProperties();

                foreach (var prop in props)
                {
                    var propName = prop.Name;
                    if (propName != "ClientUtcTime" && Program.PropertyCache.ContainsKey(propName))
                    {
                        prop.SetValue(gameEvent, Program.PropertyCache[propName]);
                    }
                }

                SendMessage(gameEvent);
            }
        }

        private static Type[] GetMessageTypes()
        {
            if (s_messageTypes == null)
            {
                var assembly = typeof(INetherMessage).GetTypeInfo().Assembly;
                var types = assembly.GetTypes();
                var gameEventTypes =
                    from t in types
                    where typeof(INetherMessage).IsAssignableFrom(t) && t != typeof(INetherMessage)
                    orderby t.Name
                    select t;

                s_messageTypes = gameEventTypes.ToArray();
            }

            return s_messageTypes;
        }

        private void TypedMessageSelected(INetherMessage gameEvent)
        {
            EditMessageProperties(gameEvent);
            SendMessage(gameEvent);
        }

        private static void SendMessage(INetherMessage gameEvent)
        {
            // Serialize object to JSON
            var msg = JsonConvert.SerializeObject(
                gameEvent,
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            EventHubManager.SendMessageToEventHub(msg).Wait();
        }

        private void EditMessageProperties(INetherMessage gameEvent)
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