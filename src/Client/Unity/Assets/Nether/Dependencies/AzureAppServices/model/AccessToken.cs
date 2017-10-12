// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Azure.AppServices
{
	[System.Serializable]
	public class AccessToken
	{
		public string access_token;

		/// <summary>
		/// Facebook, Microsoft Account, Azure Active Directory "access_token" request body
		/// </summary>
		public AccessToken (string accessTokenValue)
		{
			access_token = accessTokenValue;
		}
	}
}
