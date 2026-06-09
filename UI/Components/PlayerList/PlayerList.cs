using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jobworld
{

	public class PlayerList : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_elementPrefab;

		[SerializeField]
		private Transform m_parent;
		
		private Dictionary<long, PlayerListElement> m_elementsWithId = new();

		private IPlayerDataModel m_playerDataModel;
		
		private IUserProfileModel m_userProfileModel;

		private IChangeJobModel m_changeJobModel;

		void Awake()
		{
			m_playerDataModel = new PlayerDataModel();
			m_userProfileModel = new UserProfileModel();
			m_changeJobModel = new ChangeJobModel();
		}

		private PlayerListElement InstantiateElement(long id)
		{
			var instance = Instantiate(m_elementPrefab, m_parent).GetComponent<PlayerListElement>();
			m_elementsWithId.Add(id, instance);
			instance.jobChanged += OnJobChanged;
			return instance;
		}

		private void DestroyElement(long id)
		{
			var instance = m_elementsWithId[id];
			m_elementsWithId.Remove(id);
			instance.jobChanged -= OnJobChanged;
			Destroy(instance.gameObject);
		}

		private void OnJobChanged(long playerId, string jobName)
		{
			m_changeJobModel.ChangeJob(playerId, jobName);
		}

		public async void LoadAllPlayer()
		{
			var allPlayerData = await m_playerDataModel.GetAllPlayerData();
			foreach (var d in allPlayerData)
			{
				var element = InstantiateElement(d.authId);
				var userProfile = await m_userProfileModel.GetUserProfileDataById(d.authId);
				element.UpdateInfo(userProfile);
			}
		}
		
		public async void AddPlayer(long id)
		{
			if (!m_elementsWithId.ContainsKey(id))
			{
				var element = InstantiateElement(id);
				var userProfile = await m_userProfileModel.GetUserProfileDataById(id);
				element.UpdateInfo(userProfile);
			}
			else
			{
				Debug.LogError("같은 ID를 가진 요소가 이미 존재함. ID = " + id);
			}
		}

		public async void RemovePlayer(long id)
		{
			if (m_elementsWithId.ContainsKey(id))
			{
				DestroyElement(id);
			}
			else
			{
				Debug.LogError("해당 ID를 가진 요소가 이미 존재함. ID = " + id);

			}
		}
	}

}