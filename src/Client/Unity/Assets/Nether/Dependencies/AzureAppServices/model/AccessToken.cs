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
