using System.Threading.Tasks;

namespace Jobworld
{

	public interface IPlayerDataModel
	{
		Task<SessionPlayerData> GetPlayerDataById(long id); 
		
		Task<SessionPlayerData[]> GetAllPlayerData();
	}

}