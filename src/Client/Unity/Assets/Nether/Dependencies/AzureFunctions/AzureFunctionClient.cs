using Azure.App;
using RestClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azure.Functions {
  public sealed class AzureFunctionClient : Client {
    private string key;

    public User User { get; private set; }

    public AzureFunctionClient(string url, string key = null) : base(url) {
      this.key = key;
    }

    public static AzureFunctionClient Create(string account, string key = null) {
      string url = AppUrl(account);
      return new AzureFunctionClient(url, key);
    }

    public static string AppUrl(string account) {
      return string.Format("https://{0}.azurewebsites.net", account);
    }

    public bool HasKey() {
      if (!string.IsNullOrEmpty(key)) {
        return true;
      }
      return false;
    }

    public string GetKey() {
      return key;
    }

    /// <summary>
    /// Client-directed single sign on (POST with access token)
    /// </summary>
    /*
public IEnumerator Login(AuthenticationProvider authenticationProvider, string token, Action<IRestResponse<User>> callback = null)
    {
        string provider = authenticationProvider.ToString().ToLower();
        string url = string.Format("{0}/.auth/login/{1}", Url, provider);
        Debug.Log("Login Request Url: " + url + " access token: " + token);
        ZumoRequest request = new ZumoRequest(this, url, Method.POST);
        request.AddBodyAccessToken(token);
        yield return request.request.Send();
        request.ParseJson<User>(callback);
    }
    */
  }
}
