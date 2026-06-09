using System;
using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class TeacherMainLoginMenuView : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField m_idInput;
		
		[SerializeField]
		private TMP_InputField m_passwordInput;
		
		public string id => m_idInput.text;
		
		public string password => m_passwordInput.text;
		
		public event Action loginAttempted;
		
		/// <summary>
		/// Called by Button
		/// </summary>
		public void Login()
		{
			loginAttempted?.Invoke();
		}
	}

}