using UnityEngine;
using RestClient;
using System.Text;
using Azure.App;

namespace Azure.AppServices
{
	public sealed class ZumoRequest : RestRequest
	{
		public ZumoRequest (Client client, string url, Method httpMethod) : base (url, httpMethod)
		{
			this.AddHeader ("ZUMO-API-VERSION", "2.0.0");
			this.AddHeader ("Accept", "application/json");
			this.AddHeader ("Content-Type", "application/json; charset=utf-8");
/* 
			if (client.User != null && !string.IsNullOrEmpty (client.User.authenticationToken)) {
				this.AddHeader ("X-ZUMO-AUTH", client.User.authenticationToken);
				Debug.Log ("Auth UserId:" + client.User.user.userId);
			}
			*/
		}

		public void AddBodyAccessToken (string token)
		{
			AccessToken accessToken = new AccessToken (token);
			this.AddBody<AccessToken> (accessToken);
		}

		public override void AddBody<T> (T data, string contentType = "application/json; charset=utf-8")
		{
			string jsonString = JsonHelper.ToJsonExcludingSystemProperties (data); // strip App Services System Properties
			byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
			this.AddBody (bytes, contentType);
		}

	}
}
