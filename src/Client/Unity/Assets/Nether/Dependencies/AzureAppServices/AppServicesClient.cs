// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
