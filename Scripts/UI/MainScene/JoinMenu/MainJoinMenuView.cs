using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jobworld
{

	public class MainJoinMenuView : MonoBehaviour
	{
		private const string STRING_FORMAT_NICKNAME = "환영합니다. {0}님!";
		
		[SerializeField]
		private TextMeshProUGUI m_nicknameField;
		
		[SerializeField]
		private TMP_InputField m_sessionNameInput;
		
		[SerializeField]
		private Button m_joinButton;
		
		public string sessionName => m_sessionNameInput.text;

		public event Action myDataOpened;

		public event Action joinSessionAttempted;

		void OnEnable()
		{
			myDataOpened?.Invoke();
		}

		public void UpdateMyData(UserData myData)
		{
			m_nicknameField.text = string.Format(STRING_FORMAT_NICKNAME, myData.nickname);
		}

		/// <summary>
		/// Called by Button
		/// </summary>
		public void JoinSession()
		{
			joinSessionAttempted?.Invoke();
		}

		public void Update()
		{
			m_joinButton.interactable = sessionName.Length == 6;
		}
	}

}