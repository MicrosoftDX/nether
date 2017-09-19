// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Nether.Web.Utilities
{
    public class SharedAccessSignatureTokenProviderEx
    {
        // TODO: Replace with SDK version, when supported from within .NET Core
        public static string GetSharedAccessSignature(string keyName, string sharedAccessKey, string resource, TimeSpan tokenTimeToLive)
        {
            var fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + tokenTimeToLive.TotalSeconds, CultureInfo.InvariantCulture);
            var stringToSign = WebUtility.UrlEncode(resource) + "\n" + expiry;
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(sharedAccessKey));

            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            var token = string.Format(CultureInfo.InvariantCulture,
                "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                WebUtility.UrlEncode(resource),
                WebUtility.UrlEncode(signature),
                expiry,
                keyName);

            return token;
        }
    }
}

