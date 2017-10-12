// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using UnityEngine;
using System.Text;
using RESTClient;

namespace Azure.AppServices {
  public sealed class ZumoRequest : RestRequest {

    // NB: Exclude system properties is a workaround to ignore read-only system properties during JSON utility deserialization.
    // If set to true then App Services System Properties will be stripped from the request body during deserialization.
    private bool excludeSystemProperties;

    public ZumoRequest(string url, Method httpMethod = Method.GET, bool excludeSystemProperties = true, AuthenticatedUser user = null) : base(url, httpMethod) {
      this.excludeSystemProperties = excludeSystemProperties;
      this.AddHeader("ZUMO-API-VERSION", "2.0.0");
      this.AddHeader("Accept", "application/json");
      this.AddHeader("Content-Type", "application/json; charset=utf-8");
      // User Authentictated request
      if (user != null && !string.IsNullOrEmpty(user.authenticationToken)) {
        this.AddHeader("X-ZUMO-AUTH", user.authenticationToken);
      }
    }

    // Facebook, Microsoft Account, Azure Active Directory
    public void AddBodyAccessToken(string token) {
      AccessToken accessToken = new AccessToken(token);
      this.AddBody<AccessToken>(accessToken);
    }

    // Twitter
    public void AddBodyAccessTokenSecret (string token, string tokenSecret)
		{
			AccessTokenSecret accessTokenSecret = new AccessTokenSecret (token, tokenSecret);
			this.AddBody<AccessTokenSecret> (accessTokenSecret);
		}

    // Google+
    public void AddBodyAccessTokenId (string token, string idToken)
		{
			AccessTokenId accessTokenId = new AccessTokenId (token, idToken);
			this.AddBody<AccessTokenId> (accessTokenId);
		}

    public override void AddBody<T>(T data, string contentType = "application/json; charset=utf-8") {
      string jsonString = excludeSystemProperties ? JsonHelper.ToJsonExcludingSystemProperties(data) : JsonUtility.ToJson(data);
      byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
      this.AddBody(bytes, contentType);
    }

  }
}
