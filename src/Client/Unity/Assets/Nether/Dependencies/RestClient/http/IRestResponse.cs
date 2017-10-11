// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;

namespace RESTClient {
  public interface IRestResponse<T> {
    bool IsError { get; }

    string ErrorMessage { get; }

    string Url { get; }

    HttpStatusCode StatusCode { get; }

    string Content { get; }

    T Data { get; }
  }
}

