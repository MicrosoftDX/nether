// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Nether.Web.Features.Identity
{
    public class FacebookGraphService
    {
        private readonly IOptions<SignInMethodOptions> _signInOptions;

        public FacebookGraphService(
            IOptions<SignInMethodOptions> signInOptions
        )
        {
            _signInOptions = signInOptions;
        }
        private static Lazy<HttpClient> s_httpClientLazy = new Lazy<HttpClient>(CreateHttpClient);
        private static HttpClient CreateHttpClient()
        {
            return new HttpClient()
            {
                BaseAddress = new Uri("https://graph.facebook.com/"),
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json")
                    }
                }
            };
        }
        public async Task<FacebookTokenDebugResult> TokenDebugAsync(string token)
        {
            // Call facebook graph api to validate the token

            // As per https://developers.facebook.com/docs/facebook-login/access-tokens/#apptokens, we're using "appid|appsecret" for the app token
            var facebookOptions = _signInOptions.Value.Facebook;
            var appToken = facebookOptions.AppId + "|" + facebookOptions.AppSecret;


            var client = s_httpClientLazy.Value;
            var response = await client.GetAsync($"/debug_token?input_token={token}&access_token={appToken}");

            dynamic body = await response.Content.ReadAsAsync<dynamic>();

            var result = ParseTokenDebugResult(body);

            return result;
        }

        public static FacebookTokenDebugResult ParseTokenDebugResult(dynamic body)
        {
            FacebookTokenDebugResult result = new FacebookTokenDebugResult();
            if (((object)body.error) != null)
            {
                // Note that we get here if (for example) the token is for a different application
                // i.e. the endpoint validates that the app token matches
                result.Error = new FacebookError();
                result.Error.Code = (int)body.error.code;
                result.Error.Type = (string)body.error.type;
                result.Error.Message = (string)body.error.message;
            }
            else if (body.data.error != null)
            {
                result.Error = new FacebookError();
                result.Error.Code = (int)body.data.error.code;
                result.Error.Message = (string)body.data.error.message;
            }
            if (body.data != null)
            {
                result.UserId = (string)body.data.user_id;
                result.IsValid = (bool)body.data.is_valid;
                if (body.data.scopes != null)
                {
                    result.Scopes = ((JArray)body.data.scopes)
                                        .Select(i => i.Value<string>())
                                        .ToArray();
                }
                if (body.data.expires_at != null)
                {
                    result.ExpiresAt = GetDateTimeFromUnixTime((int)body.data.expires_at);
                }
            }

            return result;
        }

        private static readonly DateTime s_unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static DateTime GetDateTimeFromUnixTime(int unixTime)
        {
            return s_unixEpoch.AddSeconds(unixTime);
        }
    }
    public class FacebookTokenDebugResult
    {
        // See https://developers.facebook.com/docs/graph-api/reference/v2.10/debug_token for docs
        public string UserId { get; set; }
        public bool IsValid { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string[] Scopes { get; set; }
        public FacebookError Error { get; set; }
    }
    public class FacebookError
    {
        public int Code { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}