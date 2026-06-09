using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class RankingElement : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI m_nickname;

		[SerializeField]
		private TextMeshProUGUI m_jobTitle;

		[SerializeField]
		private TextMeshProUGUI m_dollar;

		public void UpdateProfile(UserProfile profile)
		{
			m_nickname.text = profile.nickname;
		}

		public void UpdatePlayerData(SessionPlayerData playerData)
		{
			string jobTitle = JobDatabase.instance.GetJob(playerData.jobName).jobName;
			m_jobTitle.text = jobTitle;
			m_dollar.text = playerData.dollar.ToString();
		}
	}

}