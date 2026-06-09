using System;
using System.Threading.Tasks;

namespace Jobworld
{

	public class PlayerDataModel : IPlayerDataModel
	{
//		private ISessionDataInterface m_sessionDI = new FirebaseSessionDataInterface();

		public Task<SessionPlayerData> GetPlayerDataById(long id)
		{
//			return m_sessionDI.GetPlayerDataById(id);
			throw new NotImplementedException();
		}
		
		public Task<SessionPlayerData[]> GetAllPlayerData()
		{
			throw new NotImplementedException();
//			return m_sessionDI.GetAllPlayerData();
		}
	}

}