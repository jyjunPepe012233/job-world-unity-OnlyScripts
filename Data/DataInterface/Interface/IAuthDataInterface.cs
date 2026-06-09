using System.Threading.Tasks;

namespace Jobworld
{

	public interface IAuthDataInterface
	{
		Task<WWWResponse> Login(string username, string password);
	}

}