using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class PlayerListElement : MonoBehaviour
	{
		public UserProfile userProfile { get; private set; }
		
		[SerializeField]
		private TextMeshProUGUI m_nicknameField;

		[SerializeField]
		private TextMeshProUGUI m_nameField;

		[SerializeField]
		private JobDropdown m_dropdown;

		public event Action<long, string> jobChanged;

		void OnEnable()
		{
			m_dropdown.onValueChanged += InvokeJobChangedEvent;
		}

		void OnDisable()
		{
			m_dropdown.onValueChanged -= InvokeJobChangedEvent;
		}

		private void InvokeJobChangedEvent()
		{
			jobChanged?.Invoke(userProfile.id, m_dropdown.selectedJob);
		}

		public void UpdateInfo(UserProfile profile)
		{
			this.userProfile = profile;
			
			m_nicknameField.text = profile.nickname;
			m_nameField.text = profile.name;
		}
	}

}