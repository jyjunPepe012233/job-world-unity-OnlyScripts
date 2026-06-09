using System.Threading.Tasks;

namespace Jobworld
{

	public interface IChangeJobModel
	{
		Task ChangeJob(long playerId, string jobName);
	}

}