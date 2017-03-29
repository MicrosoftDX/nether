// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NetherSDK.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace NetherSDK
{
    public class NetherClient : MonoBehaviour
    {
        public string NetherDeploymentUrl = "https://netherwebjppy3olypaayo.azurewebsites.net";

        private string _netherAuthorizationToken;

        public string NetherClientID = "devclient"; //from Nether deployment
        public string NetherClientSecret = "devsecret"; //from Nether deployment
        public string NetherScope = "openid profile nether-all";

        [HideInInspector]
        public Endpoint EventHubEndPoint = null;

        //singleton implementation
        private void Awake()
        {
            Instance = this;
        }
        public static NetherClient Instance;

        private void Start()
        {
            Globals.DebugFlag = true;
        }

        public void GetPlayer(Action<CallbackResponse<Player>> callback)
        {
            Utilities.ValidateForNull(callback);
            StartCoroutine(CallNetherServiceInternal<Player>(null, GetNetherURL<Player>(), HttpMethod.Get, _netherAuthorizationToken, callback));
        }

        public void PutPlayer(Player instance, Action<CallbackResponse<Player>> callback)
        {
            Utilities.ValidateForNull(instance, callback);
            StartCoroutine(CallNetherServiceInternal<Player>(instance, GetNetherURL<Player>(), HttpMethod.Put, _netherAuthorizationToken, callback));
        }

        public void GetLeaderboards(Action<CallbackResponse<Leaderboard[]>> callback)
        {
            Utilities.ValidateForNull(callback);
            StartCoroutine(CallNetherServiceInternal<Leaderboard[]>(null, GetNetherURL<Leaderboard[]>(), HttpMethod.Get, _netherAuthorizationToken, callback));
        }

        public void GetLeaderboardNamed(string leaderboardName, Action<CallbackResponse<LeaderboardNamed>> callback)
        {
            Utilities.ValidateForNullOrEmpty(leaderboardName);
            Utilities.ValidateForNull(callback);
            StartCoroutine(CallNetherServiceInternal<LeaderboardNamed>(null, GetNetherURL<Leaderboard[]>() + "/" + leaderboardName,
                HttpMethod.Get, _netherAuthorizationToken, callback, false));
        }

        public void PostScore(Score instance, Action<CallbackResponse<Score>> callback)
        {
            Utilities.ValidateForNull(instance, callback);
            StartCoroutine(CallNetherServiceInternal<Score>(instance, GetNetherURL<Score>(), HttpMethod.Post, _netherAuthorizationToken, callback));
        }

        public void GetEndpoint(Action<CallbackResponse<Endpoint>> callback)
        {
            Utilities.ValidateForNull(callback);
            StartCoroutine(CallNetherServiceInternal<Endpoint>(null, GetNetherURL<Endpoint>(), HttpMethod.Get, _netherAuthorizationToken, callback, false));
        }

        public void PostData<T>(T instance, Action<CallbackResponse<T>> callback)
            where T : class, new()
        {
            Utilities.ValidateForNull(instance, EventHubEndPoint);
            Utilities.ValidateForNullOrEmpty(EventHubEndPoint.authorization, EventHubEndPoint.url);
            StartCoroutine(CallNetherServiceInternal<T>(instance, EventHubEndPoint.url, HttpMethod.Post, EventHubEndPoint.authorization, callback));
        }

        private IEnumerator CallNetherServiceInternal<T>(T instanceToPostOrPut, string url, HttpMethod method, string authorizationToken,
            Action<CallbackResponse<T>> onCallNetherServiceCompleted, bool containsResultProperty = true)
            where T : class
        {
            if (string.IsNullOrEmpty(_netherAuthorizationToken))
            {
                CallbackResponse<T> response = new CallbackResponse<T>();
                response.Status = CallBackResult.Unauthorized;
                response.Exception = new Exception("Missing Nether authorization token");
                onCallNetherServiceCompleted(response);
                yield break;
            }

            string jsonToPostOrPut = (instanceToPostOrPut != null) ? JsonUtility.ToJson(instanceToPostOrPut) : null;
            using (UnityWebRequest www = Utilities.BuildNetherServiceWebRequest(url, method.ToString(), jsonToPostOrPut, authorizationToken))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);
                CallbackResponse<T> response = new CallbackResponse<T>();
                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else
                {
                    //if we have a response body
                    if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                    {
                        try
                        {
                            if (containsResultProperty)//e.g. we get a result of PlayerResult instance that contains a Player field with the actual object we want
                            {
                                string typename = GetNetherTypeinResponse<T>();

                                //hate this
                                object dataResult = JsonUtility.FromJson(www.downloadHandler.text, Type.GetType(string.Format("{0}.{1}{2}", "NetherSDK.Models", typename, "Result")));
                                //and this
                                response.Result = (T)dataResult.GetType().GetField(typename.ToLower()).GetValue(dataResult);
                            }
                            else
                            {
                                //just deserialize into T
                                response.Result = JsonUtility.FromJson<T>(www.downloadHandler.text);
                            }
                            response.Status = CallBackResult.Success;
                        }
                        catch (Exception ex)
                        {
                            response.Status = CallBackResult.DeserializationFailure;
                            response.Exception = ex;
                        }
                    }
                    else
                    {
                        response.Status = CallBackResult.Success;
                    }
                }
                onCallNetherServiceCompleted(response);
            }
        }

        private string GetNetherURL<T>()
        {
            return string.Format("{0}/api/{1}", NetherDeploymentUrl, GetNetherTypeinResponse<T>().ToLower());
        }

        private string GetNetherTypeinResponse<T>()
        {
            string name = string.Empty;
            if (typeof(Array).IsAssignableFrom(typeof(T))) //check if T is Array
                name = typeof(T).Name.Replace("[]", "s");
            else if (typeof(T) == typeof(Score) || typeof(T) == typeof(Leaderboard))
                name = typeof(T).Name + "s";
            else
                name = typeof(T).Name;

            return name;
        }

        public void GetNetherAccessToken(string facebookUserAccessToken, Action<string> callback)
        {
            Utilities.ValidateForNull(facebookUserAccessToken, callback);
            StartCoroutine(GetAccessTokenInternal(facebookUserAccessToken, callback));
        }

        private object _heartBeatInstanceToSend = null;
        public void StartHeartbeat(float time, float repeatRate, object heartBeatInstanceToSend)
        {
            if (time <= 0f && repeatRate <= 0f)
            {
                throw new Exception("Time and repeatRate should be greater than zero");
            }
            Utilities.ValidateForNull(heartBeatInstanceToSend);

            if (Globals.DebugFlag)
                Debug.Log("HeartBeatAction starting");

            //this.heartBeatInstanceToSend holds a reference to the object that we will send
            _heartBeatInstanceToSend = heartBeatInstanceToSend;
            InvokeRepeating("HeartbeatAction", time, repeatRate);
        }

        public void StopHeartbeat()
        {
            if (Globals.DebugFlag)
                Debug.Log("HeartBeatAction stopping");
            CancelInvoke("HeartbeatAction");
        }

        private void HeartbeatAction()
        {
            PostData(_heartBeatInstanceToSend, (x) =>
            {
                if (Globals.DebugFlag)
                    Debug.Log("HeartBeatAction completed");
            });
        }

        private IEnumerator GetAccessTokenInternal(string facebookUserAccessToken, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField("client_id", NetherClientID);
            form.AddField("client_secret", NetherClientSecret);
            form.AddField("scope", NetherScope);
            form.AddField("token", facebookUserAccessToken);
            form.AddField("grant_type", "fb-usertoken");

            using (UnityWebRequest req = UnityWebRequest.Post(NetherDeploymentUrl + "/identity/connect/token", form))
            {
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader(Globals.Content_Type, Globals.ApplicationFormUrlEncoded);

                yield return req.Send();

                var authResult = JsonUtility.FromJson<NetherAuthResult>(req.downloadHandler.text);
                _netherAuthorizationToken = authResult.token_type + " " + authResult.access_token;
            }
            callback(_netherAuthorizationToken);
        }
    }
}
