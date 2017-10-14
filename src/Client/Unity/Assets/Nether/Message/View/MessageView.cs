// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageView : MonoBehaviour {
  public Text message;

  [SerializeField]
  private RectScale transition;

  public void Open() {
    if (transition != null) {
      transition.Open();
      return;
    }
  }

  public void Close() {
    if (transition != null && transition.enabled) {
      transition.Close();
      return;
    }
    Destroy(this.gameObject);
  }
}
