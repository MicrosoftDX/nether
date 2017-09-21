using UnityEngine;
using System.Collections;
using System;
using RestClient;

namespace Azure.Functions
{
	public class AzureFunction
	{
		private AzureFunctionClient client;

		private string name; // function name

        private const string API = "api/";

        public AzureFunction (string name, AzureFunctionClient client)
		{
			this.client = client;
			this.name = name;
		}

		public override string ToString ()
		{
			return name;
		}

		public IEnumerator Post<B,T> (B body, Action<IRestResponse<T>> callback = null) 
		{
			RestRequest request = new RestRequest(ApiUrl(), Method.POST);
			if (client.HasKey()) {
				request.AddQueryParam("code", client.GetKey(), true);
			}
			Debug.Log ("POST Request URL: " + request.Request.url);
			request.AddBody (body);
			yield return request.Request.Send ();
			if ( typeof(T) == typeof(string) ) {
                Debug.Log("> TEXT Response");
				request.GetText<T> (callback);
			}
            else
            {
                Debug.Log("> JSON Response");
                request.ParseJson<T>(callback);
            }
		}

        public IEnumerator Get<T>(Action<IRestResponse<T[]>> callback = null)
        {
            RestRequest request = new RestRequest(ApiUrl(), Method.GET);
            if (client.HasKey())
            {
                request.AddQueryParam("code", client.GetKey(), true);
            }
            Debug.Log("GET Request URL: " + request.Request.url);
            yield return request.Request.Send();
            request.ParseJsonArray<T>(callback);
        }

        private string ApiUrl()
        {
            return string.Format("{0}/{1}{2}", client.Url, API, name);
        }


    }
}