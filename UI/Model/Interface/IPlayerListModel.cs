using System;

namespace Jobworld
{

	public interface IPlayerListModel
	{
		event Action<long> memberAdded;
		
		event Action<long> memberUpdated;
		
		event Action<long> memberRemoved;
	}

}