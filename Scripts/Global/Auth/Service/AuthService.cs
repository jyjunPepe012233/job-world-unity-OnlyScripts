using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{

	public static class AuthService
	{
		private static IAuthDataInterface m_authDI = DataInterfaceContainer.auth;
		
		private static IUserDataInterface m_userDI = DataInterfaceContainer.user;

		public static event Action loginSucceed;

		public static event Action<ErrorResponse> loginFailed;
		
		public static async Task Login(string username, string password)
		{
			var response = await m_authDI.Login(username, password);

			LoginResponse tokens = null;
			switch (response.GetStatus())
			{
				case -1: // 로그인 성공 시 json 응답에 "status" 필드가 없으므로 GetStatus()의 반환이 -1임
					tokens = response.Parse<LoginResponse>();
					break;
				
				default:
					loginFailed?.Invoke(response.Parse<ErrorResponse>());
					break;
			}

			if (tokens == null)
			{
				Debug.LogError("[AuthService] Login 응답에 문제가 발생했습니다");
			}
			
			LocalAuthorizationHolder.accessToken = tokens.accessToken;
			LocalAuthorizationHolder.refreshToken = tokens.refreshToken;
			
			var userData = await m_userDI.GetMyData();
			LocalAuthorizationHolder.id = userData.Child("data").Parse<UserData>().id;
			
			loginSucceed?.Invoke();
		}
	}

}