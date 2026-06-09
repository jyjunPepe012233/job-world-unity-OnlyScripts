namespace Jobworld
{

	public class CurrentSessionDataModel : ICurrentSessionDataModel
	{
		public SessionData GetCurrentSessionData()
		{
			return MasterClientLocalInfoHolder.sessionData;
		}
	}

}