using NetherSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace NetherSDK
{
    public static class Utilities
    {
        public static void ValidateForNull(params object[] objects)
        {
            foreach (object obj in objects)
            {
                if (obj == null)
                {
                    throw new Exception("Argument null");
                }
            }

        }

        /// <summary>
        /// Builds and returns a UnityWebRequest object
        /// </summary>
        /// <param name="url">Url to hit</param>
        /// <param name="method">POST,GET, etc.</param>
        /// <param name="json">Any JSON to send</param>
        /// <param name="authenticationToken">Authentication token for the headers</param>
        /// <returns>A UnityWebRequest object</returns>
        public static UnityWebRequest BuildNetherServiceWebRequest(string url, string method, string json, string authenticationToken)
        {
            UnityWebRequest www = new UnityWebRequest(url, method);

            www.SetRequestHeader(Globals.Accept, Globals.ApplicationJson);
            www.SetRequestHeader(Globals.Content_Type, Globals.ApplicationJson);


            if (!string.IsNullOrEmpty(authenticationToken))
                www.SetRequestHeader(Globals.Authorization, authenticationToken.Trim());

            www.downloadHandler = new DownloadHandlerBuffer();

            if (!string.IsNullOrEmpty(json))
            {
                byte[] payload = Encoding.UTF8.GetBytes(json);
                UploadHandler handler = new UploadHandlerRaw(payload);
                handler.contentType = Globals.ApplicationJson;
                www.uploadHandler = handler;
            }
            return www;
        }


        public static bool IsWWWError(UnityWebRequest www)
        {
            return www.isError || (www.responseCode >= 400L && www.responseCode <= 511L);
        }

        public static void BuildResponseObjectOnFailure(CallbackResponse response, UnityWebRequest www)
        {
            if (www.responseCode == 404L)
                response.Status = CallBackResult.NotFound;
            else if (www.responseCode == 409L)
                response.Status = CallBackResult.ResourceExists;
            else
                response.Status = CallBackResult.Failure;

            string errorMessage = www.error;
            if (errorMessage == null && www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                errorMessage = www.downloadHandler.text;
            else
                errorMessage = Globals.ErrorOccurred;

            Exception ex = new Exception(errorMessage);
            response.Exception = ex;

            //add Nether error
            if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
            {
                response.NetherError = JsonUtility.FromJson<NetherErrorResult>(www.downloadHandler.text);
            }
        }

        public static void BuildResponseObjectOnException(CallbackResponse response, Exception ex)
        {
            response.Status = CallBackResult.LocalException;
            response.Exception = ex;
        }


    }

    //http://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/#post-2585129
    public class JsonHelper
    {
        public static T[] GetJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array = null;
        }

        [Serializable]
        private class TableStorageResult<T>
        {
            public T[] value = null;
        }

        public static T[] GetJsonArrayFromTableStorage<T>(string json)
        {
            TableStorageResult<T> result = JsonUtility.FromJson<TableStorageResult<T>>(json);
            return result.value;
        }
    }

    public enum HttpMethod
    {
        Post,
        Get,
        Patch,
        Delete,
        Put,
        Merge
    }
}
