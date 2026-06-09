using System.Collections.Generic;
using Oculus.Interaction;
using Photon.Pun;
using UnityEngine;

namespace Jobworld
{

	public class NetworkedPlayer : MonoBehaviour
	{
		private static Dictionary<int, NetworkedPlayer> m_playersById;

		public static Dictionary<int, NetworkedPlayer> playerById => m_playersById;
		
		public PhotonView view;
		
		public GameObject[] onlyLocalActive;

		public GameObject[] onlyRemoteActive;

		public NicknameLabel nicknameLabel;
		
		[Optional]
		public PlayerAvatarManager avatarManager;
		
		public void Start()
		{
			foreach (var i in onlyLocalActive)
			{
				i.SetActive(view.IsMine);
			}
			
			foreach (var i in onlyRemoteActive)
			{
				i.SetActive(!view.IsMine);
			}
			
			nicknameLabel.SetNickname(view.Owner.NickName);
			nicknameLabel.gameObject.SetActive(!view.IsMine);
		}
		
		public void SetJob(string jobName)
		{
			JobSO jobSo = JobDatabase.instance.GetJob(jobName);
			
			SetAvatarBasedOnJob(jobSo);
		}

		private void SetAvatarBasedOnJob(JobSO jobSo)
		{
			if (avatarManager != null)
			{
				if (jobSo != null)
				{
					avatarManager.SetAvatar(jobSo.avatarSetting);
				}
				else
				{
					avatarManager.ResetAvatarToDefault();
				}
			}
		}
		
	}

}