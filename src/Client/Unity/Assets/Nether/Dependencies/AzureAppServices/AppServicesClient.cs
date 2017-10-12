using RESTClient;

namespace Azure.AppServices {
  public sealed class AppServicesClient : ZumoClient {

    public AppServicesClient(string url) : base(url) {
    }

    public static AppServicesClient Create(string account) {
      string url = AppUrl(account);
      return new AppServicesClient(url);
    }

  }
}
