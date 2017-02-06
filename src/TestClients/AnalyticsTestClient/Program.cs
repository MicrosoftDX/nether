using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Nether.Analytics.GameEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnalyticsTestClient
{
    public class MainMenu
    {
        
    }

    public class Program
    {
        private static string _connectionString = "";
        private static string _ehName = "";

        private static Dictionary<string, object> _defaultValues = new Dictionary<string, object>();
        public static void Main(string[] args)
        {
            SetupDefaultValues();
            MainMenu();
        }

        private static void SetupDefaultValues()
        {
            _defaultValues = new Dictionary<string, object>
            {
                {"GameSessionId", Guid.NewGuid().ToString()},
                {"EventCorrelationId", Guid.NewGuid(). ToString()},
                {"GamerTag", "krist00fer" }
            };
        }

        private static void MainMenu()
        {
            var menu = new Menu("Analytics Test Client - Main Menu",
                    new MenuItem('1', "SetupMenu", SetupMenu),
                    new MenuItem('2', "Send Known Message", SendKnownMessageMenu),
                    new MenuItem('3', "Send Custom Message", SendCustomMessage),
                    new MenuItem('0', "Exit", () => { })
                );

            menu.Show();
        }

        private static void SendKnownMessageMenu()
        {
            var menuItems = new List<MenuItem>();
            var gameEventTypes = GetGameEventTypes();

            var menuKey = 'a';
            foreach (var gameEventType in gameEventTypes)
            {
                var gameEvent = (IGameEvent) Activator.CreateInstance(gameEventType);
                menuItems.Add(new MenuItem(menuKey++, $"{gameEvent.GameEventType}, {gameEvent.Version}", () => { FillPropertiesAndSend(gameEvent); }));
            }

            menuItems.Add(new MenuItem('0', "Main Menu", MainMenu));
            var menu = new Menu("Analytics Test Client - Send Known Message", menuItems.ToArray());

            //foreach (var gameEventType in gameEventTypes)
            //{
            //    Console.WriteLine(gameEventType.Name);

            //    var t = (IGameEvent)Activator.CreateInstance(gameEventType);
            //    Console.WriteLine(t.GameEventType);
            //}

            //var menu = new Menu("Analytics Test Client - Send Known Message",
            //    new MenuItem('1', "game-start", FillEventTypePropertiesAndSend<GameStartEvent>),
            //    new MenuItem('2', "game-heartbeat", FillEventTypePropertiesAndSend<GameHeartbeatEvent>),
            //    new MenuItem('9', "game-stop", FillEventTypePropertiesAndSend<GameStopEvent>)
            //);

            menu.Show();
        }

        private static void FillPropertiesAndSend(IGameEvent ge)
        {
            Console.WriteLine("Fill out property values for Game Event. Press <Enter> to keep current/default value.");
            ge.ClientUtcTime = DateTime.UtcNow;

            //var ge = new T {ClientUtcTime = DateTime.UtcNow};

            foreach (var prop in ge.GetType().GetProperties())
            {
                var propName = prop.Name;
                if (_defaultValues.ContainsKey(propName))
                {
                    prop.SetValue(ge, _defaultValues[propName]);
                }
                var propValue = prop.GetValue(ge);

                if (propName == "GameEventType" || propName == "Version")
                {
                    // Don't ask for input for these properties, just display their 
                    // values since they are static and can't be changed
                    Console.WriteLine($"{propName}: {propValue}");
                }
                else
                {
                    Console.Write($"{propName} [{propValue}]:");
                    var answer = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(answer))
                    {
                        if (prop.PropertyType == typeof(int))
                        {
                            int i = int.Parse(answer);
                            prop.SetValue(ge, i);
                            _defaultValues[propName] = i;
                        }
                        else if (prop.PropertyType == typeof(long))
                        {
                            long l = long.Parse(answer);
                            prop.SetValue(ge, l);
                            _defaultValues[propName] = l;
                        }
                        else if (prop.PropertyType == typeof(DateTime))
                        {
                            DateTime dt = DateTime.Parse(answer);
                            prop.SetValue(ge, dt);
                            _defaultValues[propName] = dt;
                        }
                        else
                        {
                            // If nothing else, use string directly
                            prop.SetValue(ge, answer);
                            _defaultValues[propName] = answer;
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(ge);

            Console.WriteLine(json);

            SendMessageToEventHub(json).Wait();

            SendKnownMessageMenu();
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
        //private static void SendGameStart()
        //{
        //    var ev = new GameStartEvent {ClientUtcTime = DateTime.UtcNow};

        //    AskToFillGameEvent(ev);

        //    var json = JsonConvert.SerializeObject(ev);

        //    Console.WriteLine(json);
        //    Console.ReadLine();

        //    //Question.Ask(nameof(GameStartEvent.ClientUtcTime), ref ev.ClientUtcTime, DateTime.UtcNow);
        //}

        //private static void AskToFillGameEvent(IGameEvent ge)
        //{

        //    foreach (var prop in ge.GetType().GetProperties())
        //    {
        //        var propName = prop.Name;
        //        var propValue = prop.GetValue(ge);
        //        var question = new Question($"{propName} [{propValue}]:");
        //        var result = question.Ask();
        //        if (!string.IsNullOrWhiteSpace(result))
        //        {
        //            prop.SetValue(ge, result);
        //        }
        //    }
        //}

        private static void SendGameEnd()
        {
            throw new NotImplementedException();
        }

        private static void SendGameHeartbeat()
        {
            throw new NotImplementedException();
        }

        private static void SetupMenu()
        {
            var setupMenu = new Menu("Analytics Test Client - SetupMenu Menu",
                new MenuItem('1', $"ConnectionString: {_connectionString}", SetConnectionString),
                new MenuItem('2', $"Event Hub Path/Name: {_ehName}", SetEventHubName),
                new MenuItem('0', "Main Menu", MainMenu)
                );

            setupMenu.Show();
        }

        private static void SetConnectionString()
        {
            var question = new Question($"Connection String [{_connectionString}]:");
            var connectionString = question.AskOnNewLine();
            if (!string.IsNullOrWhiteSpace(connectionString))
                _connectionString = connectionString;

            SetupMenu();
        }

        private static void SetEventHubName()
        {
            var question = new Question($"Event Hub Name [{_ehName}]:");
            var ehName = question.AskOnNewLine();
            if (!string.IsNullOrWhiteSpace(ehName))
                _ehName = ehName;

            SetupMenu();
        }

        private static void SendCustomMessage()
        {
            var question = new Question("Message to send:");
            var msg = question.AskOnNewLine();

            SendMessageToEventHub(msg).Wait();

            MainMenu();
        }

        private static async Task SendMessageToEventHub(string msg)
        {
            Console.WriteLine($"Connecting to EventHub [{_ehName}]");
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(_connectionString)
            {
                EntityPath = _ehName
            };

            var client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            Console.WriteLine($"Sending message...");
            await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(msg)));
            Console.WriteLine("Message sent!");
            await client.CloseAsync();
        }
    }
}
