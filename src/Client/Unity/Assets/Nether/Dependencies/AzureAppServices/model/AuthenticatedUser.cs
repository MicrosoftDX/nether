namespace Azure.AppServices
{
	[System.Serializable]
	public class AuthenticatedUser
	{
		public string authenticationToken;
		public User user;
	}
}
