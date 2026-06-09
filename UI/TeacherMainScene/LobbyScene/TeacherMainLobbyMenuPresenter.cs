	using System;
	using System.Threading.Tasks;
	using UnityEngine;

namespace Jobworld
{

	public class TeacherMainLobbyMenuPresenter : MonoBehaviour
	{
		[SerializeField]
		private TeacherMainMenuPageNavigator m_pageNav;
		
		[SerializeField]
		private TeacherMainLobbyMenuView m_view;
		
		private ICurrentSessionDataModel m_currentSessionDataModel = new CurrentSessionDataModel();

		private IPlayerListModel m_playerListModel;

		void OnEnable()
		{
			m_playerListModel = new PlayerListModel();
			
			m_pageNav.m_lobbyMenuOpened += UpdateSessionData;

			m_view.startPlayAttempted += StartPlay;
			m_view.closeSessionAttempted += CloseSession;

			m_playerListModel.memberAdded += AddPlayerOnList;
			m_playerListModel.memberRemoved += RemovePlayerOnList;
		}

		void OnDisable()
		{
			m_pageNav.m_lobbyMenuOpened -= UpdateSessionData;

			m_playerListModel.memberAdded -= AddPlayerOnList;
			m_playerListModel.memberRemoved -= RemovePlayerOnList;
		}

		private void UpdateSessionData()
		{
			m_view.UpdateSessionData(m_currentSessionDataModel.GetCurrentSessionData());
		}

		private void AddPlayerOnList(long playerId)
		{
			m_view.AddPlayerOnList(playerId);
		}

		private void RemovePlayerOnList(long playerId)
		{
			m_view.RemovePlayerOnList(playerId);
		}

		private async void StartPlay()
		{
			var result = await MasterClientService.StartPlay();
			if (result)
			{
				m_pageNav.OpenPlayMenu();
			}
		}

		private void CloseSession()
		{
			throw new NotImplementedException();
		}
	}
}