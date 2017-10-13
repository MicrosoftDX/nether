// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Azure.AppServices
{
	[System.Serializable]
	public class AccessTokenSecret
	{
		public string access_token;
		public string access_token_secret;

		/// <summary>
		/// Twitter with "access_token" and "access_token_secret" request body
		/// </summary>
		public AccessTokenSecret (string accessTokenValue, string accessTokenSecret)
		{
			access_token = accessTokenValue;
			access_token_secret = accessTokenSecret;
		}
	}
}
