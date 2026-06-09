using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{

	public class LoadingSceneVisualManager : MonoBehaviour
	{
		[SerializeField]
		[Interface(typeof(ITargetLoadSceneNameVisual))]
		private Object m_sceneNameVisual;
		
		public ITargetLoadSceneNameVisual sceneNameVisual { get; private set; }

		[SerializeField]
		[Interface(typeof(ILoadingProgressVisual))]
		private Object m_progressVisual;

		public ILoadingProgressVisual progressVisual { get; private set; }

		public void Awake()
		{
			sceneNameVisual = m_sceneNameVisual as ITargetLoadSceneNameVisual;
			progressVisual = m_progressVisual as ILoadingProgressVisual;
		}
		
		public void OnEnable()
		{
			LoadingSceneManager.onLoadingStarted += UpdateTargetScene;
			LoadingSceneManager.onProgressUpdated += UpdateLoadingProgress;
		}

		public void OnDisable()
		{
			LoadingSceneManager.onLoadingStarted -= UpdateTargetScene;
			LoadingSceneManager.onProgressUpdated -= UpdateLoadingProgress;
		}

		private void UpdateTargetScene(string targetSceneName)
		{
			sceneNameVisual?.SetTargetSceneName(targetSceneName);
		}
		
		private void UpdateLoadingProgress(float progress)
		{
			progressVisual?.SetProgress(progress);
		}
	}

}