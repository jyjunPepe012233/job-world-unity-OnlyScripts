using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jobworld
{

	public interface ISceneLoadInfoProvider
	{
		float progress { get; }
		bool isCompleted { get; }
		string targetScene { get; }
	}

	public class LoadingSceneManager : MonoBehaviourSingleton<LoadingSceneManager>
	{
		public const string LOADING_SCENE_NAME = "LoadingScene";

		public static event Action<string> onLoadingStarted;

		public static event Action<float> onProgressUpdated;

		public static event Action onLoadingFinished;

		private LoadingSceneSettings m_settings;

		private Action m_loadStartCallback;
		
		private Action m_loadCompleteCallback;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		protected static void InitializeOnLoad()
		{
			InitializeSingleton();
		}

		public void Awake()
		{
			m_settings = Resources.Load<LoadingSceneSettings>("LoadingSceneSettings");
			if (m_settings == null) m_settings = new LoadingSceneSettings();
		}

		public Coroutine EnterLoadingScene(Func<ISceneLoadInfoProvider> sceneLoadInfoProvider, Action loadStartCallback = null, Action loadCompleteCallback = null)
		{
			m_loadStartCallback = loadStartCallback;
			m_loadCompleteCallback = loadCompleteCallback;
			
			return StartCoroutine(EnterLoadingSceneCoroutine(sceneLoadInfoProvider));
		}

		private IEnumerator EnterLoadingSceneCoroutine(Func<ISceneLoadInfoProvider> sceneLoadInfoProvider)
		{
			// LoadScene 호출 후 씬 초기화 대기
			SceneManager.LoadScene(LOADING_SCENE_NAME, LoadSceneMode.Single);
			yield return null;
			
			// 동기적 씬 로딩 후 로딩 코루틴 시작 
			ISceneLoadInfoProvider provider = sceneLoadInfoProvider.Invoke();
			yield return null;
			
			StartCoroutine(LoadingCoroutine(provider));
		}
		
		private IEnumerator LoadingCoroutine(ISceneLoadInfoProvider sceneLoadInfoProvider)
		{
			// 이벤트 호출 및 콜백 사용
			onLoadingStarted?.Invoke(sceneLoadInfoProvider.targetScene);
			m_loadStartCallback?.Invoke();
			
			// 씬 로딩 대기
			while (sceneLoadInfoProvider.progress < 0.9f && !sceneLoadInfoProvider.isCompleted)
			{
				onProgressUpdated?.Invoke(sceneLoadInfoProvider.progress * (1 - m_settings.fakeLoadingRange));
				yield return null;
			}

			// 씬 로딩이 완료되면 fake loading 진행
			float fakeLoadingPos = 0;
			while (fakeLoadingPos < 1)
			{
				fakeLoadingPos += Time.deltaTime / m_settings.fakeLoadingTime;

				float visualProgress = (1 - m_settings.fakeLoadingRange) + (m_settings.fakeLoadingProgressOverTime.Evaluate(fakeLoadingPos) * m_settings.fakeLoadingRange);
				
				onProgressUpdated?.Invoke(visualProgress);
				yield return null;
			}
			
			// 이벤트 호출 및 콜백 사용
			onLoadingFinished?.Invoke();
			m_loadCompleteCallback?.Invoke();
		}
	}

}