// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RESTClient {
  public class QueryParams {
    private Dictionary<string, string> parameters;

    public QueryParams() {
      parameters = new Dictionary<string, string>();
    }

    public void AddParam(string key, string value) {
      parameters.Add(key, value);
    }

    public override string ToString() {
      if (parameters.Count == 0) {
        return "";
      }
      StringBuilder sb = new StringBuilder("?");
      foreach (KeyValuePair<string, string> param in parameters) {
        string key = WWW.EscapeURL(param.Key);
        string value = WWW.EscapeURL(param.Value);
        sb.Append(key + "=" + value + "&");
      }
      sb.Remove(sb.Length - 1, 1);
      return sb.ToString();
    }
  }
}
