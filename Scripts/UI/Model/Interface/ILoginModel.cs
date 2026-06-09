using System;
using System.Threading.Tasks;

namespace Jobworld
{

	public interface ILoginModel
	{
		event Action loginSucceed;

		event Action<ErrorResponse> loginFailed;
		
		Task Login(string id, string password);
	}

}