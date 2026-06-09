using System.Collections.Generic;
using UnityEngine;

namespace Jobworld
{

	public class Ranking : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_elementPrefab;

		[SerializeField]
		private Transform m_parent;
		
		private Dictionary<long, RankingElement> m_elementsWithId = new();

		private IUserProfileModel m_userProfileModel;
		
		private IPlayerListModel m_playerListModel;
		
		private IPlayerDataModel m_playerDataModel;
		
		void Awake()
		{
			m_userProfileModel = new UserProfileModel();
			m_playerListModel = new PlayerListModel();
			m_playerDataModel = new PlayerDataModel();
		}

		void OnEnable()
		{
			m_playerListModel.memberAdded += OnPlayerListUpdated;
			m_playerListModel.memberRemoved += OnPlayerListUpdated;
			m_playerListModel.memberUpdated += OnPlayerInfoUpdated;
		}

		void OnDisable()
		{
			m_playerListModel.memberAdded -= OnPlayerListUpdated;
			m_playerListModel.memberRemoved -= OnPlayerListUpdated;
			m_playerListModel.memberUpdated -= OnPlayerInfoUpdated;
		}

		private void OnPlayerListUpdated(long id)
		{
			ReloadAllPlayers();
		}
		
		private async void OnPlayerInfoUpdated(long id)
		{
			var userProfile = await m_userProfileModel.GetUserProfileDataById(id);
			var playerData = await m_playerDataModel.GetPlayerDataById(id);
				
			var element = m_elementsWithId[id];
				
			element.UpdateProfile(userProfile);
			element.UpdatePlayerData(playerData);
		}

		private RankingElement InstantiateElement(long id)
		{
			var instance = Instantiate(m_elementPrefab, m_parent).GetComponent<RankingElement>();
			m_elementsWithId.Add(id, instance);
			return instance;
		}
		
		private void DestroyElement(long id)
		{
			var instance = m_elementsWithId[id];
			m_elementsWithId.Remove(id);
			Destroy(instance.gameObject);
		}

		private async void ReloadAllPlayers()
		{
			// clear old elements
			foreach (var id in m_elementsWithId.Keys)
			{
				DestroyElement(id);
			}
			
			var allPlayerData = await m_playerDataModel.GetAllPlayerData();
			foreach (var playerData in allPlayerData)
			{
				long id = playerData.authId;
				
				var element = InstantiateElement(id);
				
				var userProfile = await m_userProfileModel.GetUserProfileDataById(id);
				
				element.UpdateProfile(userProfile);
				element.UpdatePlayerData(playerData);
			}
		}
	}

}