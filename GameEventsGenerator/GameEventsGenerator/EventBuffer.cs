using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEventsGenerator
{
    internal class EventBuffer
    {
        private long eventId;
        // Sorted List does not allow duplicates. Add event id to the key and use custom comparer
        private SortedList<GameEventKey, GameEvent> events;

        internal EventBuffer()
        {
            eventId = 0;
            events = new SortedList<GameEventKey, GameEvent>(new GameEventKeyComparer());
        }

        public IEnumerable<GameEvent> GetEvents(DateTime timestamp)
        {
            while (events.Count > 0)
            {
                var e = events.First();
                if (e.Key.Timestamp <= timestamp)
                {
                    events.RemoveAt(0);
                    yield return e.Value;
                }
                else
                {
                    break;
                }
            }
        }

        public bool HasExistingLaterExitEvent(DateTime timestamp, string playerId)
        {
            foreach (var e in events)
            {
                if (e.Key.Timestamp >= timestamp && e.Value.PlayerId.Equals(playerId, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        internal void Add(DateTime timestamp, GameEvent e) {
            events.Add(new GameEventKey { Timestamp = timestamp, EventId = eventId++ }, e);
            Console.WriteLine("Added to eventBuffer: '{0}', eID:'{1}', '{2}', '{3}'", timestamp.ToString(), eventId, e.PlayerId, e.GameId);
        }

        private struct GameEventKey
        {
            public DateTime Timestamp;
            public long EventId;
        }

        private class GameEventKeyComparer : IComparer<GameEventKey>
        {
            public int Compare(GameEventKey x, GameEventKey y)
            {
                int compareTime = x.Timestamp.CompareTo(y.Timestamp);

                if (compareTime != 0)
                {
                    return compareTime;
                } else
                    return x.EventId.CompareTo(y.EventId);
            }
        }
    }
}
