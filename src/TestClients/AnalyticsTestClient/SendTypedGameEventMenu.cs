// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Nether.Analytics.MessageFormats;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AnalyticsTestClient.Utils;
using System.Threading;

namespace AnalyticsTestClient
{
    public class SendTypedGameEventMenu : ConsoleMenu
    {
        private static Type[] s_gameEventTypes = null;

        public SendTypedGameEventMenu()
        {
            Title = "Nether Analytics Test Client - Send Static Game Events";

            var gameEventTypes = GetGameEventTypes();
            var menuKey = 'a';
            foreach (var gameEventType in gameEventTypes)
            {
                var gameEvent = (INetherMessage)Activator.CreateInstance(gameEventType);
                MenuItems.Add(menuKey++,
                    new ConsoleMenuItem(
                        $"{gameEvent.Type}, {gameEvent.Version}",
                        () => StaticGameEventSelected(gameEvent)));
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

                var typeToSend = GetGameEventTypes().TakeRandom();
                var gameEvent = (INetherMessage)Activator.CreateInstance(typeToSend);

                gameEvent.ClientUtcTime = DateTime.UtcNow;

                var props = gameEvent.GetType().GetProperties();

                foreach (var prop in props)
                {
                    var propName = prop.Name;
                    if (propName != "ClientUtcTime" && Program.PropertyCache.ContainsKey(propName))
                    {
                        prop.SetValue(gameEvent, Program.PropertyCache[propName]);
                    }
                }

                SendGameEvent(gameEvent);

                //Thread.Sleep(1000);
            }
        }

        private static Type[] GetGameEventTypes()
        {
            if (s_gameEventTypes == null)
            {
                var assembly = typeof(INetherMessage).GetTypeInfo().Assembly;
                var types = assembly.GetTypes();
                var gameEventTypes =
                    from t in types
                    where typeof(INetherMessage).IsAssignableFrom(t) && t != typeof(INetherMessage)
                    orderby t.Name
                    select t;

                s_gameEventTypes = gameEventTypes.ToArray();
            }

            return s_gameEventTypes;
        }

        private void StaticGameEventSelected(INetherMessage gameEvent)
        {
            EditGameEventProperties(gameEvent);
            SendGameEvent(gameEvent);
        }

        private static void SendGameEvent(INetherMessage gameEvent)
        {
            // Serialize object to JSON
            var msg = JsonConvert.SerializeObject(
                gameEvent,
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            //var msg = JsonConvert.SerializeObject(gameEvent);
            EventHubManager.SendMessageToEventHub(msg).Wait();
        }

        private void EditGameEventProperties(INetherMessage gameEvent)
        {
            gameEvent.ClientUtcTime = DateTime.UtcNow;

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

    public static class RandomExtensions
    {
        private static Random s_random = new Random();

        public static S TakeRandom<S>(this S[] array)
        {
            if (array == null || array.Length == 0)
                return default(S);

            return array[s_random.Next(array.Length)];
        }
    }
}