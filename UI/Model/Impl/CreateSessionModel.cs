using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{

	public class CreateSessionModel : ICreateSessionModel
	{
		public event Action createSucceed;
		
		public event Action createFailed;
		
		public async Task<bool> CreateSession(SessionCreationSetting creationSetting)
		{
			var tcs = new TaskCompletionSource<bool>();

			void Success()
			{
				tcs.TrySetResult(true);
				createSucceed?.Invoke();
			}
			
			void Fail()
			{
				tcs.TrySetResult(false);
				createFailed?.Invoke();
			}

			MasterClientService.onSessionCreated += Success;
			MasterClientService.onSessionCreateFailed += Fail;
			
			MasterClientService.CreateSession(creationSetting, out string _);

			var result = await tcs.Task;
			
			MasterClientService.onSessionCreated -= Success;
			MasterClientService.onSessionCreateFailed -= Fail;

			return result;
		}
	}

}