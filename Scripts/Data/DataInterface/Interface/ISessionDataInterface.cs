using System;
using System.Threading.Tasks;

namespace Jobworld
{

	public interface ISessionDataInterface
	{
		event Action<SessionPlayerData> memberAdded;

		event Action<SessionPlayerData> memberUpdated;

		event Action<long> memberRemoved;
		
		void SubscribeSessionEvents();
		
		void UnsubscribeSessionEvents();

		Task AddSession(SessionData sessionData);

		Task RemoveSession(SessionData sessionData);
		
		Task<SessionPlayerData> GetPlayerDataById(long wasId);

		Task<SessionPlayerData[]> GetAllPlayerData();

		Task<int> CountsOfPlayerSelectedJob();

		Task AddPlayer(SessionPlayerData playData);

		Task RemovePlayer(long wasId);

		Task UpdatePlayerJob(long wasId, string jobName);
		
		Task UpdatePlayerDollar(long wasId, int dollar);
	}

}