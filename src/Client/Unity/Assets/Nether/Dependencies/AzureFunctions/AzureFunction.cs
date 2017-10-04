using UnityEngine;
using System.Collections;
using System;
using RestClient;

namespace Azure.Functions {
  public class AzureFunction {
    private AzureFunctionClient client;

    private string name; // function name

    private const string API = "api";

    public AzureFunction(string name, AzureFunctionClient client) {
      this.client = client;
      this.name = name;
    }

    public override string ToString() {
      return name;
    }

    public IEnumerator Post<B, T>(B body, Action<IRestResponse<T>> callback = null) {
      RestRequest request = new RestRequest(ApiUrl(), Method.POST);
      if (client.HasKey()) {
        request.AddQueryParam("code", client.GetKey(), true);
      }
      request.AddBody(body);
      yield return request.Request.Send();
      if (typeof(T) == typeof(string)) {
        request.GetText<T>(callback);
      } else {
        request.ParseJson<T>(callback);
      }
    }

    public IEnumerator Get<T>(string path = null, Action<IRestResponse<T[]>> callback = null) {
      RestRequest request = new RestRequest(ApiUrl(path), Method.GET);
      if (client.HasKey()) {
        request.AddQueryParam("code", client.GetKey(), true);
      }
      yield return request.Request.Send();
      request.ParseJsonArray<T>(callback);
    }

    private string ApiUrl(string path = null) {
      string endpoint = string.IsNullOrEmpty(path) ? "" : "/" + path;
      return string.Format("{0}/{1}/{2}{3}", client.Url, API, name, endpoint);
    }
  }
}
