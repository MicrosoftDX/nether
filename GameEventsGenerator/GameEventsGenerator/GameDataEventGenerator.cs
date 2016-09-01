using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace GameEventsGenerator
{
    /// <summary>
    ///  This class generates two continuous streams of game events: players starting a game and players stopping a game.
    /// </summary>
    public class GameDataEventGenerator
    {
        // Various games
        private const int MaxGameId = 5;
        // maximum session length of 300 seconds
        private const int MaxSessionLength = 300;
        private const double CrashProbability = 0.1;

        //private static readonly string[] PlayerIds = File.ReadAllText(@"..\..\Data\GamerNames.csv").Split(',');

        private static List<Player> Players;
        private static List<Location> WorldCities;
        private static int numPlayers;
        private static int numWorldCities;

        private static bool BackgroundMode;

        private readonly Random random;
        private readonly EventBuffer eventBuffer;

        public GameDataEventGenerator(Random r)
        {
            random = r;
            eventBuffer = new EventBuffer();
            Players = new List<Player>();
            WorldCities = new List<Location>();

            BackgroundMode = false;

            InitializePlayers();
        }

        /// <summary>
        /// Creates the list of world cities and players with their corresponding locations
        /// </summary>
        public void InitializePlayers()
        {
            //var reader = File.ReadAllLines(@"..\..\Data\world_cities.csv");
            var reader = File.ReadAllLines(@"..\..\Data\world_cities1.csv");
            foreach (var line in reader)
            {
                var values = line.Split(';');
                WorldCities.Add(new Location(values[0], values[2], values[3], values[1]));
            }
            numWorldCities = WorldCities.Count;
            int coordIdx = 0;

            if (BackgroundMode)
            {
                //var playerIds = File.ReadAllLines(@"..\..\Data\GamerNames.csv");
                var playerIds = File.ReadAllLines(@"..\..\Data\GamersRest.csv");
                numPlayers = playerIds.Length;

                foreach (var player in playerIds)
                {
                    coordIdx = random.Next(numWorldCities - 1);
                    Players.Add(new Player(player, WorldCities.ElementAt(coordIdx)));
                }
            }
            else
            {
                // Players from Germany (GDC taking place in Cologne, Germany)
                var germanPlayers = File.ReadAllLines(@"..\..\Data\GermanGamers.csv");
                List<Location> GermanCities = WorldCities.Where(loc => loc.Country.Equals("Germany")).ToList();
                Location Cologne = WorldCities.Where(loc => loc.City == "Cologne").First();
                int numGermanCities = GermanCities.Count;
                // percentage of players coming from Cologne
                int numColognePlayers = (int)numPlayers / 20;

                for (int i = 0; i < germanPlayers.Length; i++)
                {
                    if (i < numColognePlayers)
                    {
                        Players.Add(new Player(germanPlayers[i], Cologne));
                    }
                    else
                    {
                        coordIdx = random.Next(numGermanCities - 1);
                        Players.Add(new Player(germanPlayers[i], GermanCities.ElementAt(coordIdx)));
                    }
                }

                // Players from Brazil (GDC taking place during Olympic Games in Brazil)
                var brazilianPlayers = File.ReadAllLines(@"..\..\Data\BrazilianGamers.csv");
                List<Location> BrazilianCities = WorldCities.Where(loc => loc.Country.Equals("Brazil")).ToList();
                Location Rio = WorldCities.Where(loc => loc.City.Equals("Rio de Janeiro")).First();
                int numBrazilianCities = BrazilianCities.Count;
                // percentage of players in Rio de Janeiro
                int numRioPlayers = (int)numPlayers / 5;

                for (int i = 0; i < brazilianPlayers.Length; i++)
                {
                    if (i < numRioPlayers)
                    {
                        Players.Add(new Player(brazilianPlayers[i], Rio));
                    }
                    else
                    {
                        coordIdx = random.Next(numBrazilianCities - 1);
                        Players.Add(new Player(brazilianPlayers[i], BrazilianCities.ElementAt(coordIdx)));
                    }
                }

            }
            
            /*foreach (var playerName in playerIds)
            {
                coordIdx = random.Next(numWorldCities - 1);
                Players.Add(new Player(playerName, WorldCities.ElementAt(coordIdx)));
            }*/
            numPlayers = Players.Count;
        }

        public static GameDataEventGenerator Generator()
        {
            return new GameDataEventGenerator(new Random());
        }

        /// <summary>
        /// Generates n game events within a given interval starting at startTime
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="interval"></param>
        /// <param name="n">Number of game events</param>
        public void Next(DateTime startTime, TimeSpan interval, int n)
        {
            for (int i = 0; i < n; i++)
            {
                //var playerId = PlayerIds[random.Next(PlayerIds.Length)];
                var player = Players.ElementAt(random.Next(numPlayers - 1));
                var playerId = player.Name;
                Location playerLoc = player.PlayerLocation;

                var entryTime = startTime + TimeSpan.FromMilliseconds(random.Next((int)interval.TotalMilliseconds));
                var exitTime = entryTime + TimeSpan.FromSeconds(random.Next(MaxSessionLength));
                var crash = random.NextDouble();

                int gameId = 0;
                if (BackgroundMode)
                {
                    gameId = random.Next(MaxGameId);
                }
                if (!BackgroundMode)
                {
                    var gameProb = random.NextDouble();
                    if (gameProb > 0.7)
                    {
                        gameId = random.Next(MaxGameId - 1);
                    }
                    else
                    {
                        gameId = MaxGameId - 1;
                    }
                }

                // Only add GameEvent if there is not already an ExitEvent of given PlayerId with a timestamp greater than given timestamp
                if (!eventBuffer.HasExistingLaterExitEvent(entryTime, playerId))
                {
                    eventBuffer.Add(entryTime, new EntryEvent(playerId, gameId, entryTime, playerLoc));
                    //eventBuffer.Add(entryTime, new EntryEvent(playerId, gameId, entryTime, playerLoc.Latitude, playerLoc.Longitude));

                    if (crash > CrashProbability)
                    {
                        eventBuffer.Add(exitTime, new ExitEvent(playerId, gameId, exitTime, playerLoc));
                        //eventBuffer.Add(exitTime, new ExitEvent(playerId, gameId, exitTime, playerLoc.Latitude, playerLoc.Longitude));
                    }
                }
            }
        }

        public IEnumerable<GameEvent> GetEvents(DateTime startTime)
        {
            return eventBuffer.GetEvents(startTime);
        }
    }
}
