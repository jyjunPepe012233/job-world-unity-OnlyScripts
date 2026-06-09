using System.Threading.Tasks;

namespace Jobworld
{

	public interface IUserDataInterface
	{
		Task<WWWResponse> GetMyData();

		Task<WWWResponse> GetUserDataById(long id);
	}

}	