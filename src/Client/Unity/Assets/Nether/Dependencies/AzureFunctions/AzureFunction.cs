// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using UnityEngine;
using System.Collections;
using System;
using RESTClient;
using Azure.AppServices;

namespace Azure.Functions {
  public class AzureFunction {
    private AzureFunctionClient client;

    private string name; // Azure Function name
    private string key; // Azure Function key
    private string apiPath; // Azure Functions API path

    private const string API = "api";
    private const string PARAM_CODE = "code";

    public AzureFunction(string name, AzureFunctionClient client, string key = null, string apiPath = API) {
      this.client = client;
      this.name = name;
      this.key = key;
      this.apiPath = apiPath;
    }

    public override string ToString() {
      return name;
    }

    public IEnumerator Post<B, T>(B body, Action<IRestResponse<T>> callback = null) {
      var request = new ZumoRequest(ApiUrl(), Method.POST, false, client.User);
      if (!string.IsNullOrEmpty(key)) {
        request.AddQueryParam(PARAM_CODE, key, true);
      }
      request.AddBody(body);
      yield return request.Request.Send();
      if (typeof(T) == typeof(string)) {
        request.GetText(callback);
      } else {
        request.ParseJson<T>(callback);
      }
    }

    public IEnumerator Get<T>(string path = null, Action<IRestResponse<T[]>> callback = null) {
      var request = new ZumoRequest(ApiUrl(path), Method.GET, false, client.User);
      if (!string.IsNullOrEmpty(key)) {
        request.AddQueryParam(PARAM_CODE, key, true);
      }
      yield return request.Request.Send();
      request.ParseJsonArray<T>(callback);
    }

    private string ApiUrl(string path = null) {
      string endpoint = string.IsNullOrEmpty(path) ? "" : "/" + path;
      return string.Format("{0}/{1}/{2}{3}", client.Url, apiPath, name, endpoint);
    }
  }
}
