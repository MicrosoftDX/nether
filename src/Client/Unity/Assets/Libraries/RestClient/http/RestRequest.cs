using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace RestClient
{
	public class RestRequest : IDisposable
	{
		public UnityWebRequest Request { get; private set; }

        private QueryParams queryParams;

        public RestRequest (string url, Method method)
		{
			Request = new UnityWebRequest (url, method.ToString ());
			Request.downloadHandler = new DownloadHandlerBuffer ();
		}

		public void AddHeader (string key, string value)
		{
			Request.SetRequestHeader (key, value);
		}

		public void AddBody (byte[] bytes, string contentType)
		{
			if (Request.uploadHandler != null) {
				Debug.LogWarning ("Request body can only be set once");
				return;
			}
			Request.uploadHandler = new UploadHandlerRaw (bytes);
			Request.uploadHandler.contentType = contentType;
		}

		public virtual void AddBody<T> (T data, string contentType = "application/json; charset=utf-8")
		{
			string jsonString = JsonUtility.ToJson (data);
			byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
			this.AddBody (bytes, contentType);
		}

        public virtual void AddQueryParam (string key, string value, bool shouldUpdateRequestUrl = false)
        {
            if (queryParams == null)
            {
                queryParams = new QueryParams();
            }
            queryParams.AddParam(key, value);
            if (shouldUpdateRequestUrl)
            {
                UpdateRequestUrl();
            }
        }

        public virtual void UpdateRequestUrl()
        {
            if (queryParams == null)
            {
                Debug.Log("- No query");
                return;
            }
            var match = Regex.Match(Request.url, @"^(.+)(\\?)(.+)", RegexOptions.IgnoreCase);
            Debug.Log("* Match: " + match.Groups[0].Value );
            if (match.Groups.Count == 4 && match.Groups[0].Value.Length > 0)
            {
                string url = match.Groups[0].Value + queryParams.ToString();
                Debug.Log("Update Request URL: " + url);
                Request.url = url;
            }
        }

		#region Response and json object parsing

        private RestResult<T> GetRestResult<T>()
        {
            HttpStatusCode statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), Request.responseCode.ToString());
            RestResult<T> result = new RestResult<T>(statusCode);

            if (result.IsError)
            {
                result.ErrorMessage = "Response failed with status: " + statusCode.ToString();
                return result;
            }

            if (string.IsNullOrEmpty(Request.downloadHandler.text))
            {
                result.IsError = true;
                result.ErrorMessage = "Response has empty body";
                return result;
            }

            return result;
        }

		/// <summary>
		/// Shared method to return response result whether an object or array of objects
		/// </summary>
		private RestResult<T> TryParseJsonArray<T> ()
		{
            RestResult<T> result = GetRestResult<T>();
            // try parse an array of objects
            try
            {
                result.AnArrayOfObjects = JsonHelper.FromJsonArray<T>(Request.downloadHandler.text);
            }
            catch (Exception e)
            {
                result.IsError = true;
                result.ErrorMessage = "Failed to parse an array of objects of type: " + typeof(T).ToString() + " Exception message: " + e.Message;
            }
			return result;
		}

        private RestResult<T> TryParseJson<T>()
        {
            RestResult<T> result = GetRestResult<T>();
            // try parse an object
            try
            {
                result.AnObject = JsonUtility.FromJson<T>(Request.downloadHandler.text);
            }
            catch (Exception e)
            {
                result.IsError = true;
                result.ErrorMessage = "Failed to parse object of type: " + typeof(T).ToString() + " Exception message: " + e.Message;
            }
            return result;
        }

        /// <summary>
        /// Parses object with T data = JsonUtil.FromJson<T>, then callback RestResponse<T>
        /// </summary>
        public void ParseJson<T> (Action<IRestResponse<T>> callback = null)
		{
			RestResult<T> result = TryParseJson<T> ();

			if (result.IsError) {
				Debug.LogWarning ("Response error status:" + result.StatusCode + " code:" + Request.responseCode + " error:" + result.ErrorMessage + " Request url:" + Request.url);
				callback (new RestResponse<T> (result.ErrorMessage, result.StatusCode, Request.url, Request.downloadHandler.text));
			} else {
				callback (new RestResponse<T> (result.StatusCode, Request.url, Request.downloadHandler.text, result.AnObject));
			}
            this.Dispose();
		}

		/// <summary>
		/// Parses array of objects with T[] data = JsonHelper.GetJsonArray<T>, then callback RestResponse<T[]>
		/// </summary>
		public void ParseJsonArray<T> (Action<IRestResponse<T[]>> callback = null)
		{
			RestResult<T> result = TryParseJsonArray<T> ();

			if (result.IsError) {
				Debug.LogWarning ("Response error status:" + result.StatusCode + " code:" + Request.responseCode + " error:" + result.ErrorMessage + " Request url:" + Request.url);
				callback (new RestResponse<T[]> (result.ErrorMessage, result.StatusCode, Request.url, Request.downloadHandler.text));
			} else {
				callback (new RestResponse<T[]> (result.StatusCode, Request.url, Request.downloadHandler.text, result.AnArrayOfObjects));
			}
            this.Dispose();
        }

        /// Just return as plain text
        public void GetText<T> (Action<IRestResponse<T>> callback = null)
		{
			RestResult<string> result = GetRestResult<string>();

			if (result.IsError) {
				Debug.LogWarning ("Response error status:" + result.StatusCode + " code:" + Request.responseCode + " error:" + result.ErrorMessage + " Request url:" + Request.url);
				callback (new RestResponse<T> (result.ErrorMessage, result.StatusCode, Request.url, Request.downloadHandler.text));
			} else {
				callback (new RestResponse<T> (result.StatusCode, Request.url, Request.downloadHandler.text));
			}
            this.Dispose();
		}

        public void Dispose()
        {
            Request.Dispose(); // Request completed, clean-up resources
        }

        #endregion

    }
}
