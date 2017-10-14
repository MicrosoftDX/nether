// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RESTClient;
using Azure.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nether {
  public class MessageManager : MonoBehaviour {

    // Delegates
    public delegate void MessageSuccess(string message);
    public delegate void MessageFail();
    public static event MessageSuccess OnMessageSuccess;
    public static event MessageFail OnMessageFail;

    [Header("Azure Functions")]
    [SerializeField]
    private string account;
    [SerializeField]
    private string key;

    [Space(10)]
    [SerializeField]
    private string messageFunction = "Message";

    [Header("Unity UI")]
    [SerializeField]
    private Canvas canvas;

    [Header("Message View Prefab")]
    [SerializeField]
    private MessageView messageViewPrefab;
    private MessageView messageView;

    private AzureFunctionClient client;
    private AzureFunction messageService;

    private IEnumerator coroutine;

    private static string kTAG = "Nether";

    // Use this for initialization
    void Start() {
      if (canvas == null) {
        Debug.unityLogger.LogError(kTAG, "Attach 'canvas' property to root Canvas game object in hierarchy.");
        return;
      }

      if (messageViewPrefab == null) {
        Debug.unityLogger.LogError(kTAG, "To use the message view the associated prefabs must be attached.");
        return;
      }

      client = AzureFunctionClient.Create(account);
      messageService = new AzureFunction(messageFunction, client, key);
    }

    // Update is called once per frame
    void Update() {

    }

    public void LoadMessage() {
      coroutine = messageService.Get<string>(LoadMessageComplete);
      StartCoroutine(coroutine);
    }

    private void LoadMessageComplete(IRestResponse<string> response) {
      if (response.IsError) {
        Debug.unityLogger.LogError(kTAG, "Error load message failed. " + response.ErrorMessage);
        OnMessageFail();
        return;
      }
      // automatically open message view when loaded
      CreateMessageView(response.Content);
      OnMessageSuccess(response.Content);
    }

    #region Show/hide message view

    private void CreateMessageView(string message = "") {
      if (messageView == null) {
        messageView = Instantiate<MessageView>(messageViewPrefab, canvas.transform);
      }
      messageView.message.text = message;
      messageView.Open();
    }

    #endregion
  }
}
