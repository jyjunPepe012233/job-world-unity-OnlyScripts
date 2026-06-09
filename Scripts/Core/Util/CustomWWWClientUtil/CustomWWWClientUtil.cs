using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{

	public class CustomWWWClientUtil
	{
		public HttpClient httpClient { get; private set; }
		
		public CustomWWWClientUtil(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}
		
		private async Task<WWWResponse> SendHttpRequestMessage(HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);

			response.EnsureSuccessStatusCode();
			
			var json = await response.Content.ReadAsStringAsync();
			var data = new WWWResponse(json);
			return data;
		}

		public async Task<WWWResponse> GetAsync(string url)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, url);

			return await SendHttpRequestMessage(request);
		}

		public async Task<WWWResponse> GetWithBearerTokenAsync(string url, string accessToken)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			return await SendHttpRequestMessage(request);
		}

		private HttpRequestMessage CreatePostMessage(string url, object content)
		{
			var json = JsonUtility.ToJson(content);

			var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");

			return request;
		}

		public async Task<WWWResponse> PostAsync(string url, object content)
		{
			var request = CreatePostMessage(url, content);

			return await SendHttpRequestMessage(request);
		}
		
		public async Task<WWWResponse> PostWithBearerTokenAsync(string url, object content, string accessToken)
		{
			var request = CreatePostMessage(url, content);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			return await SendHttpRequestMessage(request);
		}
	}

}