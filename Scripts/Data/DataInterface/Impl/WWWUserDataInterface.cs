using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{

	public class WWWUserDataInterface : IUserDataInterface
	{
		private const string BASE_URL = "https://api.jobworldvr.kr";
		
		private const string URL_ME = BASE_URL + "/me";

		private const string URL_USER_BY_ID = BASE_URL + "/profile/{0}";
		
		private readonly CustomWWWClientUtil m_wwwClientUtil = new(new HttpClient());
		
		public async Task<WWWResponse> GetMyData()
		{
			var response = await m_wwwClientUtil.GetWithBearerTokenAsync(URL_ME, LocalAuthorizationHolder.accessToken);
			return response;
		}

		public async Task<WWWResponse> GetUserDataById(long id)
		{
			var response = await m_wwwClientUtil.GetWithBearerTokenAsync(string.Format(URL_USER_BY_ID, id), LocalAuthorizationHolder.accessToken);
			return response;
		}
	}

}