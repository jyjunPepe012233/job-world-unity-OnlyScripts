using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jobworld
{

	public class PlayerAvatarManager : MonoBehaviour
	{
		public PlayerRig sourceRig;
		
		[Optional, SerializeField]
		private AvatarSetting m_defaultAvatar;
		
		private AvatarSetting m_currentAvatarSetting;

		private GameObject m_avatarInstance;

		private Dictionary<AvatarSetting, GameObject> m_avatarPool = new();
		
		public void Awake()
		{
			if (m_defaultAvatar != null)
			{
				SetAvatar(m_defaultAvatar);
			}
		}

		public void ResetAvatarToDefault()
		{
			if (m_defaultAvatar != null)
			{
				SetAvatar(m_defaultAvatar);
			}
		}

		public void SetAvatar(AvatarSetting avatarSetting)
		{
			Debug.Log("PlayerAvatarManager 40: " + avatarSetting.name);
			
			if (avatarSetting == null) return;
			if (m_currentAvatarSetting == avatarSetting) return;
			
			// 현재 아바타 세팅 갱신
			m_currentAvatarSetting = avatarSetting;

			// 기존 아바타를 비활성화
			m_avatarInstance?.SetActive(false);

			// 아바타를 Instantiate.
			// avatarPool에 아바타가 이미 존재한다면 활성화만 진행.
			if (m_avatarPool.TryGetValue(avatarSetting, out GameObject instance))
			{
				instance.SetActive(true);
			}
			else
			{
				instance = Instantiate(avatarSetting.avatarPrefab, transform);

				instance.GetComponent<PlayerAvatarIKController>().sourceRig = sourceRig;
				
				m_avatarPool.Add(avatarSetting, instance);
			}

			m_avatarInstance = instance;
		}
	}

}