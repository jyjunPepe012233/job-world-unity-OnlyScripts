namespace Jobworld
{

	public static class DataInterfaceContainer
	{
		private static ISessionDataInterface m_session;
		
		public static ISessionDataInterface session
		{
			get
			{
				if (m_session == null)
					m_session = new FirebaseSessionDataInterface();

				return m_session;
			}
		}

		private static IUserDataInterface m_user;

		public static IUserDataInterface user
		{
			get
			{
				if (m_user == null)
					m_user = new WWWUserDataInterface();

				return m_user;
			}
		}

		private static IAuthDataInterface m_auth;

		public static IAuthDataInterface auth
		{
			get
			{
				if (m_auth == null)
					m_auth = new WWWAuthDataInterface();

				return m_auth;
			}
		}
	}

}