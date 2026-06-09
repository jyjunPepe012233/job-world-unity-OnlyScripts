using UnityEngine;

namespace Jobworld
{

	public class AsyncSceneLoadInfoProvider : Jobworld.ISceneLoadInfoProvider
	{
		public AsyncSceneLoadInfoProvider(string targetScene, AsyncOperation asyncLoadLevelOperation)
		{
			m_targetScene = targetScene;
			m_asyncLoadLevelOperation = asyncLoadLevelOperation;
		}
		
		private string m_targetScene;

		private AsyncOperation m_asyncLoadLevelOperation;

		public float progress => m_asyncLoadLevelOperation.progress;
		
		public bool isCompleted => m_asyncLoadLevelOperation.isDone;
		
		public string targetScene => m_targetScene;
	}

}