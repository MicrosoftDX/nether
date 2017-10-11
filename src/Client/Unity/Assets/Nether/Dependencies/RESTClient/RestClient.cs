// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;


#if !NETFX_CORE || UNITY_ANDROID
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
#endif

namespace RESTClient {
  public class RestClient {
    public string Url { get; private set; }

    /// <summary>
    /// Creates a new REST Client
    /// </summary>
    public RestClient(string url, bool forceHttps = true) {
      if (forceHttps) {
        Url = HttpsUri(url);
      }
      // required for running in Windows and Android
#if !NETFX_CORE || UNITY_ANDROID
      ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
#endif
    }

    public override string ToString() {
      return this.Url;
    }

    /// <summary>
    /// Changes 'http' to be 'https' instead
    /// </summary>
    private static string HttpsUri(string appUrl) {
      return Regex.Replace(appUrl, "(?si)^http://", "https://").TrimEnd('/');
    }

    private static string DomainName(string url) {
      var match = Regex.Match(url, @"^(https:\/\/|http:\/\/)(www\.)?([a-z0-9-_]+\.[a-z]+)", RegexOptions.IgnoreCase);
      if (match.Groups.Count == 4 && match.Groups[3].Value.Length > 0) {
        return match.Groups[3].Value;
      }
      return url;
    }

#if !NETFX_CORE || UNITY_ANDROID
    private bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
      // Check the certificate to see if it was issued from Azure
      if (certificate.Subject.Contains(DomainName(Url))) {
        return true;
      } else {
        return false;
      }
    }
#endif

  }
}
