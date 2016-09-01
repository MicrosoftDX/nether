using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;

namespace GameEventsGenerator
{
    public abstract class GameEvent
    {
        public string PlayerId { get; set; }
        public int GameId { get; set; }
        public abstract string Format();
        public Location PlayerLocation { get; set; }
    }

    public class EntryEvent : GameEvent
    {
        public DateTime EntryTime { get; set; }

        public EntryEvent(string playerId, int gameId, DateTime entryTime)
        {
            this.PlayerId = playerId;
            this.GameId = gameId;
            this.EntryTime = entryTime;
        }

        public EntryEvent(string playerId, int gameId, DateTime entryTime, Location playerLocation)
        {
            PlayerId = playerId;
            GameId = gameId;
            EntryTime = entryTime;
            PlayerLocation = playerLocation;
        }

        public override string Format()
        {
            return FormatJson();
        }

        private string FormatJson()
        {
            return JsonConvert.SerializeObject(new
            {
                PlayerId = this.PlayerId,
                GameId = this.GameId.ToString(CultureInfo.InvariantCulture),
                Time = this.EntryTime.ToString("o"),
                GameActivity = "1",
                Latitude = this.PlayerLocation.Latitude.ToString(CultureInfo.InvariantCulture),
                Longitude = this.PlayerLocation.Longitude.ToString(CultureInfo.InvariantCulture),
                City = this.PlayerLocation.City.ToString(CultureInfo.InvariantCulture),
                Country = this.PlayerLocation.Country.ToString(CultureInfo.InvariantCulture)
            });
        }

        private string FormatCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"PlayerId,GameId,Timestamp,GameActivity,Latitude,Longitude,City,Country");
            sb.AppendLine(
                string.Join(
                    ",",
                    this.PlayerId,
                    this.GameId.ToString(CultureInfo.InvariantCulture),
                    this.EntryTime.ToString("o"),
                    @"start",
                    this.PlayerLocation.Latitude.ToString(CultureInfo.InvariantCulture),
                    this.PlayerLocation.Longitude.ToString(CultureInfo.InvariantCulture),
                    this.PlayerLocation.City.ToString(CultureInfo.InvariantCulture),
                    this.PlayerLocation.Country.ToString(CultureInfo.InvariantCulture)));

            return sb.ToString();
        }
    }

    public class ExitEvent : GameEvent
    {
        public DateTime ExitTime { get; set; }

        public ExitEvent(string playerId, int gameId, DateTime exitTime)
        {
            PlayerId = playerId;
            GameId = gameId;
            ExitTime = exitTime;
        }

        public ExitEvent(string playerId, int gameId, DateTime exitTime, Location playerLocation)
        {
            PlayerId = playerId;
            GameId = gameId;
            ExitTime = exitTime;
            PlayerLocation = playerLocation;
        }

        public override string Format()
        {
            return FormatJson();
        }

        private string FormatJson()
        {
            return JsonConvert.SerializeObject(new
            {
                PlayerId = this.PlayerId,
                GameId = this.GameId.ToString(CultureInfo.InvariantCulture),
                Time = this.ExitTime.ToString("o"),
                GameActivity = "0",
                Latitude = this.PlayerLocation.Latitude.ToString(CultureInfo.InvariantCulture),
                Longitude = this.PlayerLocation.Longitude.ToString(CultureInfo.InvariantCulture),
                City = this.PlayerLocation.City.ToString(CultureInfo.InvariantCulture),
                Country = this.PlayerLocation.Country.ToString(CultureInfo.InvariantCulture)
            });
        }

        private string FormatCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"PlayerId,GameId,Timestamp,GameActivity,Latitude,Longitude,City,Country");
            sb.AppendLine(
                string.Join(
                    ",",
                    this.PlayerId,
                    this.GameId.ToString(CultureInfo.InvariantCulture),
                    this.ExitTime.ToString("o"),
                    @"stop",
                    this.PlayerLocation.Latitude.ToString(CultureInfo.InvariantCulture),
                    this.PlayerLocation.Longitude.ToString(CultureInfo.InvariantCulture),
                    this.PlayerLocation.City.ToString(CultureInfo.InvariantCulture),
                    this.PlayerLocation.Country.ToString(CultureInfo.InvariantCulture)));

            return sb.ToString();
        }
    }
}
