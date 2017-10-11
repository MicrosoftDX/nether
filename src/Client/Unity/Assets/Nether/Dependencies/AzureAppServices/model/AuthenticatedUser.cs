// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Azure.AppServices
{
	[System.Serializable]
	public class AuthenticatedUser
	{
		public string authenticationToken;
		public User user;
	}
}
