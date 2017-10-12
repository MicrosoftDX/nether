// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Azure.AppServices;
using RESTClient;

namespace Azure.Functions {
  public sealed class AzureFunctionClient : ZumoClient {

    public AzureFunctionClient(string url) : base(url) {
    }

    public static AzureFunctionClient Create(string account) {
      string url = AppUrl(account);
      return new AzureFunctionClient(url);
    }

  }
}
