using System;

namespace Azure.App
{
	[Serializable]
	public class AccessToken
	{
		public string access_token;

		/// <summary>
		/// Facebook, Google, AAD access_token request
		/// </summary>
		public AccessToken (string accessTokenValue)
		{
			access_token = accessTokenValue;
		}
	}
}