using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jobworld
{

	public static class SafeLoading
	{
		private static AsyncOperation m_asyncLoadSceneOperation;

		public static void LoadSceneWithLoading(string sceneName, Action loadStartCallback = null, Action loadCompleteCallback = null)
		{
			PhotonNetwork.IsMessageQueueRunning = false;
			PhotonNetwork.AutomaticallySyncScene = false;
			
			LoadingSceneManager.singleton.EnterLoadingScene(
				() =>
				{
					m_asyncLoadSceneOperation = SceneManager.LoadSceneAsync(sceneName);
					m_asyncLoadSceneOperation.allowSceneActivation = false;
					return new AsyncSceneLoadInfoProvider(sceneName, m_asyncLoadSceneOperation);
				},
				() => { loadStartCallback?.Invoke(); },
				() =>
				{
					m_asyncLoadSceneOperation.allowSceneActivation = true;
					PhotonNetwork.IsMessageQueueRunning = true;
					loadCompleteCallback?.Invoke();
				}
			);
		}

		public static async UniTask LoadSceneWithLoadingAsync(string sceneName, Action loadStartCallback = null, Action loadCompleteCallback = null)
		{
			bool isCompleted = false;

			void Complete()
			{
				isCompleted = true;
			}

			loadCompleteCallback += Complete;
			LoadSceneWithLoading(sceneName, loadStartCallback, loadCompleteCallback);

			while (!isCompleted)
			{
				await UniTask.Yield();
			}
		}

		public static async UniTask ChangeActiveSceneAsync(string newScene, bool unloadOldActive = false)
		{
			PhotonNetwork.IsMessageQueueRunning = false;
			PhotonNetwork.AutomaticallySyncScene = false;

			Scene oldScene = SceneManager.GetActiveScene();
			
			m_asyncLoadSceneOperation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
			m_asyncLoadSceneOperation.allowSceneActivation = false;
			
			while (m_asyncLoadSceneOperation.progress < 0.9f)
			{
				await UniTask.Yield();
			}

			// 씬 로딩이 모두 완료되면 씬을 전환
			m_asyncLoadSceneOperation.allowSceneActivation = true;
			
			Scene newS = SceneManager.GetSceneByName(newScene);
			while (!newS.isLoaded)
			{
				await UniTask.Yield();
			}
			SceneManager.SetActiveScene(newS);

			// 필요 시 기존 씬을 unload함
			if (unloadOldActive)
			{
				var unloadOperation = SceneManager.UnloadSceneAsync(oldScene);
				while (unloadOperation != null && !unloadOperation.isDone)
				{
					await UniTask.Yield();
				}
			}

			PhotonNetwork.IsMessageQueueRunning = true;
		}
	}

}