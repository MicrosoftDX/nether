// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Nether.Analytics.GameEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AnalyticsTestClient.Utils
{
    public class SendTypedGameEventMenu : ConsoleMenu
    {
        public SendTypedGameEventMenu()
        {
            Title = "Nether Analytics Test Client - Send Static Game Events";

            var gameEventTypes = GetGameEventTypes();
            var menuKey = 'a';
            foreach (var gameEventType in gameEventTypes)
            {
                var gameEvent = (IGameEvent)Activator.CreateInstance(gameEventType);
                MenuItems.Add(menuKey++,
                    new ConsoleMenuItem(
                        $"{gameEvent.Type}, {gameEvent.Version}",
                        () => StaticGameEventSelected(gameEvent)));
            }
        }

        private static Type[] GetGameEventTypes()
        {
            var assembly = typeof(IGameEvent).GetTypeInfo().Assembly;
            var types = assembly.GetTypes();
            var gameEventTypes =
                from t in types
                where typeof(IGameEvent).IsAssignableFrom(t) && t != typeof(IGameEvent)
                orderby t.Name
                select t;

            return gameEventTypes.ToArray();
        }

        private void StaticGameEventSelected(IGameEvent gameEvent)
        {
            EditGameEventProperties(gameEvent);

            // Serialize object to JSON

            var msg = JsonConvert.SerializeObject(
                gameEvent,
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            //var msg = JsonConvert.SerializeObject(gameEvent);
            EventHubManager.SendMessageToEventHub(msg).Wait();
        }

        private void EditGameEventProperties(IGameEvent gameEvent)
        {
            gameEvent.ClientUtcTime = DateTime.UtcNow;

            foreach (var prop in gameEvent.GetType().GetProperties())
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

                    //Console.Write($"{propName} [{propValue}]:");
                    //var answer = Console.ReadLine();
                    //if (!string.IsNullOrWhiteSpace(answer))
                    //{
                    //    if (prop.PropertyType == typeof(int))
                    //    {
                    //        int i = int.Parse(answer);
                    //        prop.SetValue(gameEvent, i);
                    //        Program.PropertyCache[propName] = i;
                    //    }
                    //    else if (prop.PropertyType == typeof(long))
                    //    {
                    //        long l = long.Parse(answer);
                    //        prop.SetValue(gameEvent, l);
                    //        Program.PropertyCache[propName] = l;
                    //    }
                    //    else if (prop.PropertyType == typeof(DateTime))
                    //    {
                    //        DateTime dt = DateTime.Parse(answer);
                    //        prop.SetValue(gameEvent, dt);
                    //        Program.PropertyCache[propName] = dt;
                    //    }
                    //    else
                    //    {
                    //        // If nothing else, use string directly
                    //        prop.SetValue(gameEvent, answer);
                    //        Program.PropertyCache[propName] = answer;
                    //    }
                    //}
                }
            }
        }
        //private static void EditGameEventPropertiesAndSend(IGameEvent ge)
        //{
        //    Console.WriteLine("Fill out property values for Game Event. Press <Enter> to keep current/default value.");
        //    ge.ClientUtcTime = DateTime.UtcNow;

        //    //var gameEvent = new T {ClientUtcTime = DateTime.UtcNow};

        //    foreach (var prop in ge.GetType().GetProperties())
        //    {
        //        var propName = prop.Name;
        //        if (PropertyCache.ContainsKey(propName))
        //        {
        //            prop.SetValue(ge, PropertyCache[propName]);
        //        }
        //        var propValue = prop.GetValue(ge);

        //        if (propName == "GameEventType" || propName == "Version")
        //        {
        //            // Don't ask for input for these properties, just display their 
        //            // values since they are static and can't be changed
        //            Console.WriteLine($"{propName}: {propValue}");
        //        }
        //        else
        //        {
        //            Console.Write($"{propName} [{propValue}]:");
        //            var answer = Console.ReadLine();
        //            if (!string.IsNullOrWhiteSpace(answer))
        //            {
        //                if (prop.PropertyType == typeof(int))
        //                {
        //                    int i = int.Parse(answer);
        //                    prop.SetValue(ge, i);
        //                    PropertyCache[propName] = i;
        //                }
        //                else if (prop.PropertyType == typeof(long))
        //                {
        //                    long l = long.Parse(answer);
        //                    prop.SetValue(ge, l);
        //                    PropertyCache[propName] = l;
        //                }
        //                else if (prop.PropertyType == typeof(DateTime))
        //                {
        //                    DateTime dt = DateTime.Parse(answer);
        //                    prop.SetValue(ge, dt);
        //                    PropertyCache[propName] = dt;
        //                }
        //                else
        //                {
        //                    // If nothing else, use string directly
        //                    prop.SetValue(ge, answer);
        //                    PropertyCache[propName] = answer;
        //                }
        //            }
        //        }
        //    }

        //    var json = JsonConvert.SerializeObject(ge);

        //    Console.WriteLine(json);

        //    SendMessageToEventHub(json).Wait();

        //    SendKnownMessageMenu();
        //}

    }
}