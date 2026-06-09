using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jobworld
{

	public class MainLoginMenuView : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField m_idInput;

		[SerializeField]
		private TMP_InputField m_passwordInput;

		public string id => m_idInput.text;

		public string password => m_passwordInput.text;

		public event Action loginAttempted;
		
		public event Action openJobworldWebAttempted;

		public void Login()
		{
			Debug.Log("login");
			
			loginAttempted?.Invoke();
		}

		public void OpenJobworldWeb()
		{
			openJobworldWebAttempted?.Invoke();
		}
	}

}