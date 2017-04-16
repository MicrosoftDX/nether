// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NetherSDK;
using NetherSDK.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetherHeartbeat : MonoBehaviour
{
    public Text StatusText;
    public CustomGameInfo gameInfo;

    // Use this for initialization
    private void Start()
    {
        Debug.Log("Enabling GPS");
        Input.location.Start(); //start GPS
        this.InvokeRepeating("PostGPSInfoToEventHub", 1.0f, 1.0f);
    }


    private void PostGPSInfoToEventHub()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            gameInfo.latitude = Input.location.lastData.latitude;
            gameInfo.longitude = Input.location.lastData.longitude;
            gameInfo.timestamp = Input.location.lastData.timestamp;

            NetherClient.Instance.PostData(gameInfo, (x) =>
            {
                if (Globals.DebugFlag)
                    Debug.Log("HeartBeatAction completed");
            });
        }
    }



    public void StopHeartbeat()
    {
        if (Globals.DebugFlag)
            Debug.Log("HeartBeatAction stopping");
        CancelInvoke("PostGPSInfoToEventHub");
    }


    [Serializable]
    public class CustomGameInfo
    {
        public float latitude;
        public float longitude;
        public double timestamp;
    }
}
