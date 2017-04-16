// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Include Facebook namespace
using Facebook.Unity;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Globalization;
using NetherSDK;
using UnityEngine.UI;
using NetherSDK.Models;

public class UIScript : MonoBehaviour
{
    public Text StatusText;

    // Awake function from Unity's MonoBehavior
    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }



    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            //foreach (string perm in aToken.Permissions)
            //{
            //    Debug.Log(perm);
            //}
            NetherClient.Instance.GetNetherAccessToken(aToken.TokenString, s =>
            {
                Debug.Log("Now you can safely call Nether services :). Got token " + s);
            });
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }


    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    //swagger https://netherwebjppy3olypaayo.azurewebsites.net/api/swagger/ui/index.html
    //string url = "https://netherwebjppy3olypaayo.azurewebsites.net";

    public void StartHeartBeatAction()
    {
        this.gameObject.GetComponent<NetherHeartbeat>().enabled = true;
    }

    public void PostScoreAction()
    {
        NetherClient.Instance.PostScore(new Score() { country = "Greece", score = 50 }, result =>
        {
            if (result.Status == CallBackResult.Success)
            {
                Debug.Log("SUCCESS posting score");
                StatusText.text = "SUCCESS posting score";
            }
            else
            {
                Debug.Log(result.Exception.Message);
                StatusText.text = result.Exception.Message;
                if (result.NetherError != null)
                    Debug.Log(result.NetherError.ToString());
            }
        });
    }


    public void GetPlayerAction()
    {
        NetherClient.Instance.GetPlayer(result =>
        {
            if (result.Status == CallBackResult.Success)
            {
                Debug.Log(result.Result.gamertag.ToString());
                StatusText.text = result.Result.gamertag.ToString();
            }
            else
            {
                Debug.Log(result.Exception.Message);
                StatusText.text = result.Exception.Message;
            }
        });
    }

    public void PutPlayerAction()
    {
        NetherClient.Instance.PutPlayer(new Player()
        {
            gamertag = "dgkanatsios",
            country = "Greece",
            customTag = "myCustomTag"
        }, result =>
        {
            if (result.Status == CallBackResult.Success)
            {
                Debug.Log("SUCCESS putting player");
                StatusText.text = "SUCCESS putting player";
            }
            else
            {
                Debug.Log(result.Exception.Message);
                StatusText.text = result.Exception.Message;
                if (result.NetherError != null)
                    Debug.Log(result.NetherError.ToString());
            }
        });
    }

    public void GetNamedLeaderboardAction()
    {
        NetherClient.Instance.GetLeaderboardNamed("Default", result =>
        {
            if (result.Result != null)
            {
                Debug.Log(JsonUtility.ToJson(result.Result));
            }
            else
                Debug.Log("no leaderboard available with that name");
        });
    }

    public void GetLeaderboardsAction()
    {
        NetherClient.Instance.GetLeaderboards(result =>
        {
            if (result.Result != null && result.Result.Length > 0)
            {
                foreach (var item in result.Result)
                {
                    Debug.Log(JsonUtility.ToJson(item));
                }
            }
            else
                Debug.Log("no leaderboards available");
        });
    }

    public void GetEndpointAction()
    {
        NetherClient.Instance.GetEndpoint(result =>
        {
            if (result.Result != null)
            {
                Debug.Log(result.Result.url);
                var endpoint = result.Result;
                NetherClient.Instance.EventHubEndPoint = endpoint;
            }
            else
                Debug.Log("no endpoint available");
        });
    }

    public void PostDataAction()
    {
        NetherClient.Instance.PostData(new DeviceCapabilities() { cpu = "ARM", ram = "2 GB" }, result =>
        {
            if (result.Status == CallBackResult.Success)
            {
                Debug.Log("SUCCESS posting to event hub");
                StatusText.text = "SUCCESS posting to event hub";
            }
            else
            {
                Debug.Log(result.Exception.Message);
                StatusText.text = result.Exception.Message;
                if (result.NetherError != null)
                    Debug.Log(result.NetherError.ToString());
            }
        });
    }
}


