using System;

namespace Jobworld
{

	public static class MasterClientLocalInfoHolder
	{
		public static SessionData sessionData { get; private set; }
		
		public static void HostSession(SessionData session)
		{
			sessionData = session;
		}

		public static void ClearSession()
		{
			sessionData = null;
		}

		public static string sessionName => sessionData.sessionName;
		
		public static bool isHosting => !String.IsNullOrEmpty(sessionName);
		
	}

}