// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RESTClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Net;
using UnityEngine.Assertions;

namespace Azure.AppServices {
  public abstract class ZumoClient : RestClient, IZumoClient {

    public AuthenticatedUser User { get; private set; }

    public ZumoClient(string url) : base(url) {
    }

    public static string AppUrl(string account) {
      return string.Format("https://{0}.azurewebsites.net", account);
    }

    /// <summary>
    /// Client-directed single sign on for Facebook (using access token)
    /// </summary>
    public IEnumerator LoginWithFacebook(string accessToken, Action<IRestResponse<AuthenticatedUser>> callback = null) {
      return Login(AuthenticationProvider.Facebook, accessToken, callback);
    }

    /// <summary>
    /// Client-directed single sign on for Twitter (using access token and access token secret)
    /// </summary>
    public IEnumerator LoginWithTwitter(string accessToken, string accessTokenSecret, Action<IRestResponse<AuthenticatedUser>> callback = null) {
      string url = AppServiceAuthenticationProviderUrl(AuthenticationProvider.Twitter);
      var request = new ZumoRequest(url, Method.POST, false);
      request.AddBodyAccessTokenSecret(accessToken, accessTokenSecret);
      yield return request.Request.Send();
      LoggedIn(request, callback);
    }

    /// <summary>
    /// Client-directed single sign on for Google+ (using access token and id token)
    /// </summary>
    public IEnumerator LoginWithGoogle(string accessToken, string idToken, Action<IRestResponse<AuthenticatedUser>> callback = null) {
      string url = AppServiceAuthenticationProviderUrl(AuthenticationProvider.Google);
      var request = new ZumoRequest(url, Method.POST, false);
      request.AddBodyAccessTokenId(accessToken, idToken);
      yield return request.Request.Send();
      LoggedIn(request, callback);
    }

    /// <summary>
    /// Client-directed single sign on for Microsoft Account (using access token)
    /// </summary>
    public IEnumerator LoginWithMicrosoftAccount(string accessToken, Action<IRestResponse<AuthenticatedUser>> callback = null) {
      return Login(AuthenticationProvider.MicrosoftAccount, accessToken, callback);
    }

    /// <summary>
    /// Client-directed single sign on for Azure Active Directory (using access token)
    /// </summary>
    public IEnumerator LoginWithAAD(string accessToken, Action<IRestResponse<AuthenticatedUser>> callback = null) {
      return Login(AuthenticationProvider.AAD, accessToken, callback);
    }

    private IEnumerator Login(AuthenticationProvider authenticationProvider,
                              string accessToken,
                              Action<IRestResponse<AuthenticatedUser>> callback = null) {
      string url = AppServiceAuthenticationProviderUrl(authenticationProvider);
      var request = new ZumoRequest(url, Method.POST, false);
      request.AddBodyAccessToken(accessToken);
      yield return request.Request.Send();
      LoggedIn(request, callback);
    }

    public IEnumerator Logout(Action<IRestResponse<string>> callback = null) {
      if (User == null) {
        Debug.LogWarning("Error, requires user login.");
        yield break;
      }
      string url = string.Format("{0}/.auth/logout", Url);
      var request = new ZumoRequest(url, Method.POST, false, User);
      yield return request.Request.Send();
      if (callback == null) {
        yield break;
      }
      HttpStatusCode statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), request.Request.responseCode.ToString());
      RestResponse<string> response = new RestResponse<string>(statusCode, request.Request.url, request.Request.downloadHandler.text);
      if (!statusCode.Equals(HttpStatusCode.OK)) {
        Debug.LogWarning("Error, logout request failed.");
        yield break;
      }
      // Detect result of logout webpage (using title tag to verify sign out success) with callback response
      var match = Regex.Match(request.Request.downloadHandler.text, @"<title>(.+)<\/title>", RegexOptions.IgnoreCase);
      if (match.Groups.Count == 2 && !string.IsNullOrEmpty(match.Groups[1].Value)) {
        string title = match.Groups[1].Value;
        Assert.IsTrue(string.Equals(title, "You have been signed out"));
        response = new RestResponse<string>(statusCode, request.Request.url, title);
      }
      callback(response);
      User = null;
      request.Dispose();
    }

    private string AppServiceAuthenticationProviderUrl(AuthenticationProvider authenticationProvider) {
      return string.Format("{0}/.auth/login/{1}", Url, authenticationProvider.ToString().ToLower());
    }

    private void LoggedIn(ZumoRequest request, Action<IRestResponse<AuthenticatedUser>> callback = null) {
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
      request.Dispose();
    }

  }
}
