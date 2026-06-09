using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{

	public class JoinModelBehaviour : MonoBehaviour, IJoinModel
	{
		public event Action joinSucceed;
		
		public event Action joinFailed;

		public async Task<bool> JoinSession(string sessionName)
		{ 
			var tcs = new TaskCompletionSource<bool>();

			void Success()
			{
				tcs.TrySetResult(true);
				joinSucceed?.Invoke();
			}
			
			void Fail()
			{
				tcs.TrySetResult(false);
				joinFailed?.Invoke();
			}

			if (sessionName.Length != 6)
			{
				joinSucceed?.Invoke();
				tcs.TrySetResult(false);
				return await tcs.Task;
			}
			
			GuestClientService.onJoined += Success;
			GuestClientService.onJoinFailed += Fail;
			
			GuestClientService.JoinSession(sessionName);

			var result = await tcs.Task;
			
			GuestClientService.onJoined -= Success;
			GuestClientService.onJoinFailed -= Fail;

			return result;
		}
	}

}