using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Jobworld
{

	public class WWWAuthDataInterface : IAuthDataInterface
	{
		private const string BASE_URL = "https://api.jobworldvr.kr";
		
		private const string URL_LOGIN = BASE_URL + "/login";

		private readonly CustomWWWClientUtil m_wwwClientUtil = new(new HttpClient());

		public event Action loginSucceed;
		public event Action<ErrorResponse> loginFailed;

		public async Task<WWWResponse> Login(string username, string password)
		{
			var loginRequest = new LoginRequest()
			{
				nickname = username,
				password = password
			}; 
			var loginTask = m_wwwClientUtil.PostAsync(URL_LOGIN, loginRequest);
			var timeoutTask = Task.Delay(2000);

			var completedTask = await Task.WhenAny(loginTask, timeoutTask);
			if (completedTask == loginTask)
			{
				return loginTask.IsCompletedSuccessfully
					? loginTask.Result
					: new WWWResponse(JObject.FromObject(ErrorResponse.RequestTimeout()));
			}
			else
			{
				Debug.Log("Login Timeout!!");
				return new WWWResponse(JObject.FromObject(ErrorResponse.RequestTimeout()));
			}
		}
	}

}