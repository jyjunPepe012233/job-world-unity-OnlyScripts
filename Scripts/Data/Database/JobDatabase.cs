using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jobworld
{

	[CreateAssetMenu(fileName = "JobDataBase", menuName = "Jobworld/Database/Job List")]
	public class JobDatabase : ScriptableObject
	{
		private static JobDatabase m_instance;

		public static JobDatabase instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = Resources.Load<JobDatabase>("JobDatabase");
				}
				return m_instance;
			}
		}
		
		public JobSO[] jobList;

		public JobSO GetJob(string jobName)
		{
			JobSO job = jobList.FirstOrDefault(i => i.jobName.Equals(jobName));
			
			if (job == null)
			{
				Debug.LogWarning(jobName + " 으로 명명된 JobSO를 Database에서 찾을 수 없음.");
			}

			return job;
		}
	}

}