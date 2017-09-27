using System;

namespace Azure.App
{
	[Serializable]
	public class AuthUser
	{
		public string authenticationToken;
		public User user;
	}
}