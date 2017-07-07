// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether.Ingest;
using Nether.Ingest.MessageFormats;
using System;
using System.Threading.Tasks;

namespace Nether.Test.ConsoleClient
{
    public class GeoBot : NetherBot
    {
        protected GeoBotOptions _geoOptions;
        private IGeoRouteProvider _geoRouteProvider;

        private GeoRoute _geoRoute;
        private bool _endOfRoute = false;
        private DateTime _lastGeoLocationMsgSent;

        public GeoBot(IAnalyticsClient client, IGamerTagProvider gamerTagProvider, IGameSessionProvider gameSessionProvider, IGeoRouteProvider geoRouteProvider,
            NetherBotOptions options = null, GeoBotOptions geoOptions = null) : base(client, gamerTagProvider, gameSessionProvider, options)
        {
            _geoOptions = geoOptions ?? new GeoBotOptions();
            _geoRouteProvider = geoRouteProvider;
        }

        protected async override Task OnStartAsync(IAnalyticsClient client, DateTime now)
        {
            await base.OnStartAsync(client, now);

            _lastGeoLocationMsgSent = now + TimeSpan.Zero.WithRandomDeviation(3);
            _geoRoute = _geoRouteProvider.GetGeoRoute();
        }

        protected async override Task OnTickAsync(IAnalyticsClient client, DateTime now, TimeSpan timeAlive, TimeSpan sinceLastTick, long tickNo)
        {
            await base.OnTickAsync(client, now, timeAlive, sinceLastTick, tickNo);

            if (_geoOptions.GeoLocationMsgInterval > TimeSpan.Zero && now - _lastGeoLocationMsgSent >= _geoOptions.GeoLocationMsgInterval.WithRandomDeviation(1))
            {
                var currentPosition = _geoRoute.GetPosition(timeAlive);

                if (currentPosition == null)
                {
                    // We've reached the end of the route for the bot
                    _endOfRoute = true;
                    return;
                }

                var msg = new GeoLocation
                {
                    GameSession = GameSession,
                    Lat = currentPosition.Latitude,
                    Lon = currentPosition.Longitude
                };

                await client.SendMessageAsync(msg, now);

                _lastGeoLocationMsgSent = now;
            }
        }

        protected override bool CheckSessionEnded(DateTime now)
        {
            return _endOfRoute;
        }
    }

    public class GeoBotOptions
    {
        public TimeSpan GeoLocationMsgInterval { get; set; } = TimeSpan.FromSeconds(30);
    }
}
