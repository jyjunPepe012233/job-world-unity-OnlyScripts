using System;
using System.Threading.Tasks;

namespace Jobworld
{

	public interface IJoinModel
	{
		event Action joinSucceed;

		event Action joinFailed;
		
		Task<bool> JoinSession(string sessionName);
	}

}