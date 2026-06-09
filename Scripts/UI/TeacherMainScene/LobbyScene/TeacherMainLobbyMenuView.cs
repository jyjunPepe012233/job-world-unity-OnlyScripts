using System;
using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class TeacherMainLobbyMenuView : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI m_sessionCodeField;

		[SerializeField]
		private RectTransform m_playerListContent;

		[SerializeField]
		private PlayerList m_playerList;

		public event Action startPlayAttempted;

		public event Action closeSessionAttempted;
		
		/// <summary>
		/// Called by Button
		/// </summary>
		public void CloseSession()
		{
			closeSessionAttempted?.Invoke();
		}
		
		/// <summary>
		/// Called by Button
		/// </summary>
		public void StartPlay()
		{
			startPlayAttempted?.Invoke();
		}

		public void UpdateSessionData(SessionData sessionData)
		{
			m_sessionCodeField.text = sessionData.sessionName;
		}

		public void AddPlayerOnList(long id)
		{
			m_playerList.AddPlayer(id);
		}

		public void RemovePlayerOnList(long id)
		{
			m_playerList.RemovePlayer(id);
		}
	}

}