namespace Jobworld
{

	public static class GuestClientLocalInfoHolder
	{
		public static string sessionName { get; private set; }
		
		public static SessionPlayerData playData { get; private set; }
		
		public static void JoinSession(string session)
		{
			sessionName = session;
			playData = new SessionPlayerData(LocalAuthorizationHolder.id);
		}
		
		public static void ClearSession()
		{
			sessionName = null;
			playData = null;
		}

		public static bool isJoined => !string.IsNullOrEmpty(sessionName);
	}

}