using RESTClient;
using System;
using System.Collections;

namespace Azure.AppServices {
  public interface IZumoClient {

    AuthenticatedUser User { get; }
    IEnumerator LoginWithFacebook(string accessToken, Action<IRestResponse<AuthenticatedUser>> callback = null);
    IEnumerator LoginWithTwitter(string accessToken, string accessTokenSecret, Action<IRestResponse<AuthenticatedUser>> callback = null);

    IEnumerator LoginWithGoogle(string accessToken, string idToken, Action<IRestResponse<AuthenticatedUser>> callback = null);

    IEnumerator LoginWithMicrosoftAccount(string accessToken, Action<IRestResponse<AuthenticatedUser>> callback = null);

    IEnumerator LoginWithAAD(string accessToken, Action<IRestResponse<AuthenticatedUser>> callback = null);

    IEnumerator Logout(Action<IRestResponse<string>> callback = null);
  }
}
