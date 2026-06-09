using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{
	public class LoginModelBehaviour : MonoBehaviour, ILoginModel
	{
		public event Action loginSucceed;
		
		public event Action<ErrorResponse> loginFailed;

		void OnEnable()
		{
			AuthService.loginSucceed += InvokeLoginSucceed;
			AuthService.loginFailed += InvokeLoginFailed;
		}
		
		void OnDisable()
		{
			AuthService.loginSucceed -= InvokeLoginSucceed;
			AuthService.loginFailed -= InvokeLoginFailed;
		}

		private void InvokeLoginSucceed()
		{
			Debug.Log("LoginSucceedInvoked");
			loginSucceed?.Invoke();
		}

		private void InvokeLoginFailed(ErrorResponse errorResponse)
		{
			loginFailed?.Invoke(errorResponse);
		}

		public async Task Login(string id, string password)
		{
			await AuthService.Login(id, password);
		}
	}

}