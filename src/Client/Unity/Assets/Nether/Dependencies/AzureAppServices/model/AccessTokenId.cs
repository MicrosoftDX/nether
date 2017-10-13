// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Azure.AppServices
{
	[System.Serializable]
	public class AccessTokenId
	{
		public string access_token;
    public string id_token;

		/// <summary>
		/// Google+ "access_token" and "id_token" request body
		/// </summary>
		public AccessTokenId (string accessTokenValue, string idTokenValue)
		{
			access_token = accessTokenValue;
      id_token = idTokenValue;
		}
	}
}
