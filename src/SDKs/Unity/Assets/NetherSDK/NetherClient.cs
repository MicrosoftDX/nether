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

        private string Url = "https://netherwebjppy3olypaayo.azurewebsites.net";
        public static NetherClient Instance;
        private string NetherAuthorizationToken;

        private void Awake()
        {
            Instance = this;
        }



        public void GetPlayer(Action<CallbackResponse<Player>> callback)
        {
            Utilities.ValidateForNull(callback);
            StartCoroutine(CallNetherServiceInternal<Player>(null, HttpMethod.Get, callback));
        }

        public void PutPlayer(Player instance, Action<CallbackResponse<Player>> callback)
        {
            Utilities.ValidateForNull(instance, callback);
            StartCoroutine(CallNetherServiceInternal<Player>(instance, HttpMethod.Put, callback));
        }

        public void GetLeaderboards(Action<CallbackResponse<Leaderboards[]>> callback)
        {
            Utilities.ValidateForNull(callback);
            StartCoroutine(CallNetherServiceInternal<Leaderboards[]>(null, HttpMethod.Get, callback));
        }

        public void PostScore(Score instance, Action<CallbackResponse<Score>> callback)
        {
            Utilities.ValidateForNull(instance, callback);
            StartCoroutine(CallNetherServiceInternal<Score>(instance, HttpMethod.Post, callback));
        }


        private IEnumerator CallNetherServiceInternal<T>(T instance, HttpMethod method, Action<CallbackResponse<T>> onCallNetherServiceCompleted)
            where T: class
        {
            if(string.IsNullOrEmpty(NetherAuthorizationToken))
            {
                CallbackResponse<T> response = new CallbackResponse<T>();
                response.Status = CallBackResult.Unauthorized;
                response.Exception = new Exception("Missing Nether authorization token");
                onCallNetherServiceCompleted(response);
                yield break;
            }

            string url = GetNetherURL<T>();
            string json = (instance != null) ? JsonUtility.ToJson(instance) : null;
            using (UnityWebRequest www = Utilities.BuildNetherServiceWebRequest(url, method.ToString(), json, NetherAuthorizationToken))
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
                    if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                    {
                        try
                        {
                            //we get a result of PlayerResult instance that contains a Player field with the actual object we want
                            //hate this
                            object dataResult = JsonUtility.FromJson(www.downloadHandler.text, Type.GetType(string.Format("{0}.{1}{2}", "NetherSDK.Models", typeof(T).Name, "Result")));
                            //and this
                            response.Result = (T)dataResult.GetType().GetField(typeof(T).Name.ToLower()).GetValue(dataResult);
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
            string name = typeof(T).Name.ToLower();
            return string.Format("{0}/api/{1}", Url, name);
        }

        public void GetAccessToken(string facebookUserAccessToken, Action<string> callback)
        {
            Utilities.ValidateForNull(facebookUserAccessToken, callback);
            StartCoroutine(GetAccessTokenInternal(facebookUserAccessToken, callback));
        }

        private IEnumerator GetAccessTokenInternal(string facebookUserAccessToken, Action<string> callback)
        {
            const string client_id = "devclient";
            const string client_secret = "devsecret";
            const string scope = "openid profile nether-all";

            WWWForm form = new WWWForm();
            form.AddField("client_id", client_id);
            form.AddField("client_secret", client_secret);
            form.AddField("scope", scope);
            form.AddField("token", facebookUserAccessToken);
            form.AddField("grant_type", "fb-usertoken");

            using (UnityWebRequest req = UnityWebRequest.Post(Url + "/identity/connect/token", form))
            {
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader(Globals.Content_Type, Globals.ApplicationFormUrlEncoded);

                yield return req.Send();

                var authResult = JsonUtility.FromJson<NetherAuthResult>(req.downloadHandler.text);
                NetherAuthorizationToken = authResult.token_type + " " + authResult.access_token;
            }
            callback(NetherAuthorizationToken);
        }

    }
}
