// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Nether.Analytics.EventProcessor.EventTypeHandlers
{
    public class BingLocationLookupProvider : ILocationLookupProvider
    {
        private string _bingMapsKey;

        public BingLocationLookupProvider(string bingMapsKey)
        {
            _bingMapsKey = bingMapsKey;
        }

        public async Task<LocationLookupInfo> Lookup(double lat, double lon)
        {
            var client = new WebClient();
            var bingUrl = $"http://dev.virtualearth.net/REST/v1/Locations/{lat},{lon}?key={_bingMapsKey}";
            string bingResult;

            try
            {
                bingResult = await client.DownloadStringTaskAsync(bingUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while calling Bing to Lookup coordinates");
                Console.WriteLine(ex);

                return new LocationLookupInfo("BING_ERROR_COUNTRY", "BING_ERROR_DISTRICT", "BING_ERROR_CITY");
            }

            try
            {
                var json = JObject.Parse(bingResult);
                var address = json["resourceSets"][0]["resources"][0]["address"];
                var country = (string)address["countryRegion"];
                var district = (string)address["adminDistrict"];
                var city = (string)address["locality"];

                return new LocationLookupInfo(country, district, city);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while trying to parse Location Lookup results from Bing");
                Console.WriteLine(ex);

                return new LocationLookupInfo("BING_ERROR_COUNTRY", "BING_ERROR_DISTRICT", "BING_ERROR_CITY");
            }
        }
    }
}
