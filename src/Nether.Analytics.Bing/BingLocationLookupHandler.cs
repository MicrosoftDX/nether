// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;

namespace Nether.Analytics.Bing
{
    public class BingLocationLookupHandler : IMessageHandler
    {
        string _bingMapsKey;
        string _latProperty;
        string _lonProperty;

        public BingLocationLookupHandler(string bingMapsKey, string latProperty = "lat", string lonProperty = "lon")
        {
            _bingMapsKey = bingMapsKey;
            _latProperty = latProperty;
            _lonProperty = lonProperty;
        }

        public async Task<MessageHandlerResluts> ProcessMessageAsync(Message msg, string pipelineName, int idx)
        {
            //TODO: Implement cache layer that caches result based on a surounding for example use a geohash if present
            //TODO: Replace all console logging of exceptions to generic log solution

            string lat;
            string lon;

            try
            {
                lat = msg.Properties[_latProperty];
                lon = msg.Properties[_lonProperty];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to find required properties: '{_latProperty}' and '{_lonProperty}' on message");
                return MessageHandlerResluts.FailStopProcessing;
            }

            var bingUrl = $"http://dev.virtualearth.net/REST/v1/Locations/{lat},{lon}?key={_bingMapsKey}";
            string bingResult;

            try
            {
                //var client = new WebClient();
                //bingResult = await client.DownloadStringTaskAsync(bingUrl);
                var client = new HttpClient();
                bingResult = await client.GetStringAsync(bingUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while calling Bing to Lookup coordinates");
                Console.WriteLine(ex);

                return MessageHandlerResluts.FailStopProcessing;
            }

            try
            {
                var json = JObject.Parse(bingResult);

                var address = json["resourceSets"][0]["resources"][0]["address"];
                var country = (string)address["countryRegion"];
                var district = (string)address["adminDistrict"];
                var city = (string)address["locality"];

                msg.Properties.Add("country", country);
                msg.Properties.Add("district", district);
                msg.Properties.Add("city", city);

                return MessageHandlerResluts.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while trying to parse Location Lookup results from Bing");
                Console.WriteLine(ex);

                return MessageHandlerResluts.FailStopProcessing;
            }
        }
    }
}
