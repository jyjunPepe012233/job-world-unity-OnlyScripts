using System.Threading.Tasks;

namespace Jobworld
{

	public class ChangeJobModel : IChangeJobModel
	{
		public async Task ChangeJob(long playerId, string jobName)
		{
			await MasterClientService.ChangeJob(playerId, jobName);
		}
	}

}