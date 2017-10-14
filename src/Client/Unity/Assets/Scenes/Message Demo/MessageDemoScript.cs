// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nether;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDemoScript : MonoBehaviour {

  [Header("Nether")]
  [SerializeField]
  private MessageManager messageManager;

  // Use this for initialization
  void Start() {
    if (messageManager == null) {
      Debug.LogError("Unity is missing connections to game objects in hierarchy.");
      return;
    }

    messageManager.LoadMessage();
  }

  // Update is called once per frame
  void Update() {

  }

  #region Event handlers (optional)

    void OnEnable() {
      MessageManager.OnMessageSuccess += OnMessageSuccess;
      MessageManager.OnMessageFail += OnMessageFail;
    }


    void OnDisable() {
      MessageManager.OnMessageSuccess -= OnMessageSuccess;
      MessageManager.OnMessageFail -= OnMessageFail;
    }

    private void OnMessageSuccess(string message) {
      Debug.Log(":) " + message);
    }

    private void OnMessageFail() {
      Debug.LogWarning(":(");
    }

    #endregion
}
