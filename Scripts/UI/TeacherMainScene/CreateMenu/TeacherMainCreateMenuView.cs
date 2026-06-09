using System;
using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class TeacherMainCreateMenuView : MonoBehaviour
	{
		private const string STRING_FORMAT_NICKNAME = "안녕하세요. {0} 선생님";
		
		[SerializeField]
		private TextMeshProUGUI m_nicknameField;

		[SerializeField]
		private IntSliderCreateOption m_maxPlayerOption;

		[SerializeField]
		private IntSliderCreateOption m_playTimeOption;

		public int maxPlayer => m_maxPlayerOption.GetValue();

		public int playTime => m_playTimeOption.GetValue() * 60;
		
		public event Action createAttempted;
		
		/// <summary>
		/// Called By Button
		/// </summary>
		public void CreateSession()
		{
			createAttempted?.Invoke();
		}

		public void UpdateMyData(UserData myData)
		{
			m_nicknameField.text = string.Format(STRING_FORMAT_NICKNAME, myData.nickname);
		}
	}

}