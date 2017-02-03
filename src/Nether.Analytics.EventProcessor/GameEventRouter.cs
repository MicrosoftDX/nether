using System;
using System.Collections.Generic;

namespace Nether.Analytics.EventProcessor
{
    public class GameEventRouter
    {
        private readonly Func<string, string> _gameEventTypeResolver;

        private readonly Dictionary<string, Action<string, string>> _gameEventHandlers;

        private Action<string, string> _unknownGameEventTypeHandler;
        private Action<string> _unknownGameEventFormatHandler;

        public GameEventRouter(Func<string, string> gameEventTypeResolver)
        {
            _gameEventTypeResolver = gameEventTypeResolver;

            _gameEventHandlers = new Dictionary<string, Action<string, string>>();

            // Set unknown game event and unknown game event format handlers to default handlers
            _unknownGameEventTypeHandler = (t, d) => Console.WriteLine($"Unknown Game Event ({t}): {d}");
            _unknownGameEventFormatHandler = d => Console.WriteLine($"Unknown Game Event Format: {d}");
        }

        public void RegisterKnownGameEventTypeHandler(string gameEventType, Action<string, string> action)
        {
            _gameEventHandlers.Add(gameEventType, action);
        }

        public void RegisterUnknownGameEventHandler(Action<string, string> action)
        {
            _unknownGameEventTypeHandler = action;
        }

        public void RegisterUnknownGameEventFormatHandler(Action<string> action)
        {
            _unknownGameEventFormatHandler = action;
        }

        public void HandleGameEvent(string data)
        {
            string type;

            try
            {
                type = _gameEventTypeResolver(data);
            }
            catch (Exception e)
            {
                // Resolving game event type failed. Unknown game event format?!?!
                Console.WriteLine($"An exception occurred while resolving game event type. Game event format is of unexpected type or gameEventTypeResolver is faulty: {data}");
                Console.WriteLine(e);
                _unknownGameEventFormatHandler(data);
                return;
            }

            // Get correct game event handler from dictionary of registered handlers
            var handler = _gameEventHandlers[type];

            // Check if game event type is known
            if (handler == null)
            {
                Console.WriteLine($"An unknown game event type has been encountered ({type}): {data}");
                _unknownGameEventTypeHandler(type, data);
                return;
            }

            // Pass game event data to handler
            handler(type, data);
        }
    }
}