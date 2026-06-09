using UnityEngine;

namespace Jobworld
{

	public class SessionPlayerData
	{
		public long authId;
		
		public string jobName;

		public int dollar;

		public bool hasJob => jobName == null || jobName.Length != 0;

		public SessionPlayerData(long authId)
		{
			this.authId = authId;
		}
	}

}