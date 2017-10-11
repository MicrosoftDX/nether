// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Azure.AppServices;
using RESTClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Azure.Functions {
  public sealed class AzureFunctionClient : RestClient {

    public AuthenticatedUser User { get; private set; }

    public AzureFunctionClient(string url) : base(url) {
    }

    public static AzureFunctionClient Create(string account) {
      string url = AppUrl(account);
      return new AzureFunctionClient(url);
    }

    public static string AppUrl(string account) {
      return string.Format("https://{0}.azurewebsites.net", account);
    }

    /// <summary>
    /// Client-directed single sign on (POST with access token)
    /// </summary>
    public IEnumerator Login(AuthenticationProvider authenticationProvider,
                              string token,
                              Action<IRestResponse<AuthenticatedUser>> callback = null) {
      string provider = authenticationProvider.ToString().ToLower();
      string url = string.Format("{0}/.auth/login/{1}", Url, provider);
      var request = new ZumoRequest(url, Method.POST, false);
      request.AddBodyAccessToken(token);
      yield return request.Request.Send();
      string message = request.Request.downloadHandler.text;
      IRestResponse<AuthenticatedUser> response = request.ParseJson<AuthenticatedUser>();
      if (!response.IsError) {
        User = response.Data;
        if (callback != null) {
          callback(response);
        }
      } else {
        Debug.LogWarning("Login error message:" + message);
      }
    }
  }
}
