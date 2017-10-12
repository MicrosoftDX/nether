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
