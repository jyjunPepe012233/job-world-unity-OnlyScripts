using System.Threading.Tasks;

namespace Jobworld
{

	public interface IMyDataModel
	{
		Task<UserData> GetMyData();	
	}

}