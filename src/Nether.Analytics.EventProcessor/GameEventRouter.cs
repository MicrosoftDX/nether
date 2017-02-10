// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Nether.Analytics.EventProcessor
{
    /// <summary>
    /// Routes Game Events depending on their Game Event Type to correct and registered
    /// Event Type Actions. Any Game Events with unknown Event Type or Event Formats will
    /// be routed to registered actions for handling as well.
    /// </summary>
    public class GameEventRouter
    {
        private readonly Func<string, string> _gameEventTypeResolver;
        private readonly Dictionary<string, Action<string, string>> _gameEventTypeActions;
        private Action<string, string> _unknownGameEventTypeHandler;
        private Action<string> _unknownGameEventFormatHandler;
        private Action _flushHandler;

        public GameEventRouter(Func<string, string> gameEventTypeResolver,
            Action<string> unknownGameEventFormatHandler = null,
            Action<string, string> unknownGameEventTypeHandler = null,
            Action flushHandler = null)
        {
            _gameEventTypeResolver = gameEventTypeResolver;
            _unknownGameEventFormatHandler = unknownGameEventFormatHandler;
            _unknownGameEventTypeHandler = unknownGameEventTypeHandler;
            _flushHandler = flushHandler;

            _gameEventTypeActions = new Dictionary<string, Action<string, string>>();
        }

        /// <summary>
        /// Registeres Game Event Types and what action to take if received
        /// </summary>
        /// <param name="gameEventType">Game Event Type</param>
        /// <param name="action">Action(gameEventType, data) to be called when Game Event is received</param>
        public void RegisterKnownGameEventTypeHandler(string gameEventType, Action<string, string> action)
        {
            _gameEventTypeActions.Add(gameEventType, action);
        }

        /// <summary>
        /// Handles a Game Event by looking at the Game Event Type and routing the event data to the
        /// correct registered action.
        /// </summary>
        /// <param name="data">Game Event Data</param>
        public void HandleGameEvent(string data)
        {
            string type;

            try
            {
                type = _gameEventTypeResolver(data);
            }
            catch (Exception)
            {
                // Resolving game event type failed. Unknown Game Event format?!?!
                // Invoke action to handle Unknown Game Event Formats if registered (not null)
                _unknownGameEventFormatHandler?.Invoke(data);

                // No more can be done, so return
                return;
            }


            // Check if game event type is known
            if (!_gameEventTypeActions.ContainsKey(type))
            {
                // No registered action found for the found Game Event Type
                // Invoke action to handle Unknown Game Event Type if registered (not null)
                _unknownGameEventTypeHandler?.Invoke(type, data);

                // No more can be done, so return
                return;
            }

            // Get correct game event handler from dictionary of registered handlers
            var handler = _gameEventTypeActions[type];
            // Pass game event data to correct action
            handler(type, data);
        }

        public void Flush()
        {
            _flushHandler();
        }
    }
}