using System;
using System.Threading.Tasks;

namespace Jobworld
{

	public interface ICreateSessionModel
	{
		event Action createSucceed;

		event Action createFailed;
		
		Task<bool> CreateSession(SessionCreationSetting creationSetting);
	}

}