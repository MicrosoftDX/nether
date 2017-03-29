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
        this.InvokeRepeating("GetInfoFromGPS", 1.0f, 1.0f);

        NetherClient.Instance.StartHeartbeat(2.0f, 2.0f, gameInfo);
    }


    public void GetInfoFromGPS()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            gameInfo.latitude = Input.location.lastData.latitude;
            gameInfo.longitude = Input.location.lastData.longitude;
            gameInfo.timestamp = Input.location.lastData.timestamp;
        }
    }

    [Serializable]
    public class CustomGameInfo
    {
        public float latitude;
        public float longitude;
        public double timestamp;
    }
}
