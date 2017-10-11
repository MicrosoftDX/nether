// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace RESTClient {
  public abstract class Response {
    public bool IsError { get; set; }

    public string ErrorMessage { get; set; }

    public string Url { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public string Content { get; set; }

    protected Response(HttpStatusCode statusCode) {
      this.StatusCode = statusCode;
      this.IsError = !((int)statusCode >= 200 && (int)statusCode < 300);
    }

    // success
    protected Response(HttpStatusCode statusCode, string url, string text) {
      this.IsError = false;
      this.Url = url;
      this.ErrorMessage = null;
      this.StatusCode = statusCode;
      this.Content = text;
    }

    // failure
    protected Response(string error, HttpStatusCode statusCode, string url, string text) {
      this.IsError = true;
      this.Url = url;
      this.ErrorMessage = error;
      this.StatusCode = statusCode;
      this.Content = text;
    }
  }

  public sealed class RestResponse : Response {
    // success
    public RestResponse(HttpStatusCode statusCode, string url, string text) : base(statusCode, url, text) {
    }

    // failure
    public RestResponse(string error, HttpStatusCode statusCode, string url, string text) : base(error, statusCode, url, text) {
    }
  }

  public sealed class RestResponse<T> : Response, IRestResponse<T> {
    public T Data { get; set; }

    // success
    public RestResponse(HttpStatusCode statusCode, string url, string text, T data) : base(statusCode, url, text) {
      this.Data = data;
    }
    public RestResponse(HttpStatusCode statusCode, string url, string text) : base(statusCode, url, text) {
    }

    // failure
    public RestResponse(string error, HttpStatusCode statusCode, string url, string text) : base(error, statusCode, url, text) {
    }
  }

  /// <summary>
  /// Parsed JSON result could either be an object or an array of objects
  /// </summary>
  internal sealed class RestResult<T> : Response {
    public T AnObject { get; set; }

    public T[] AnArrayOfObjects { get; set; }

    public RestResult(HttpStatusCode statusCode) : base(statusCode) {
    }
  }

}

