// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectScale : MonoBehaviour {
  public float duration = 0.5f;
  private float a = 0.0f;
  private float b = 1.0f;
  private float timer = 0.0f;
  private RectTransform panel;
  private bool open = false;
  private bool isPlaying = false;

  // Use this for initialization
  void Start() {
    panel = gameObject.GetComponent<RectTransform>();

    if (panel == null) {
      Debug.LogWarning("Game object doesn't have Rect Transform component to scale.");
      return;
    }

    ApplyInitialTransform();
  }

  // Update is called once per frame
  void Update() {
    if (!isPlaying) {
      return;
    }
    timer += Time.deltaTime;
    if (timer < duration) {
      float t = timer / duration;
      float value = Mathf.Lerp(a, b, t);
      panel.localScale = new Vector3(value, value, value);
    } else {
      ApplyFinalTransform();
      timer = 0.0f;
      isPlaying = false;
    }
  }

  public void Open() {
    open = true;
    a = 0.0f;
    b = 1.0f;
    timer = 0.0f;
    isPlaying = true;
  }

  public void Close() {
    open = false;
    a = 1.0f;
    b = 0.0f;
    timer = 0.0f;
    isPlaying = true;
  }

  public void Toggle() {
    open = !open;
    if (open) {
      Open();
    } else {
      Close();
    }
  }

  private void ApplyInitialTransform() {
    panel.localScale = Vector3.zero;
  }

  private void ApplyFinalTransform() {
    if (!open) {
      Destroy(this.gameObject); // automatically destroy after closing
      return;
    }
    panel.localScale = open ? Vector3.one : Vector3.zero;
  }

}
