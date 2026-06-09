using UnityEngine;

namespace Jobworld
{

	[CreateAssetMenu(menuName = "Jobworld/Loading Scene Settings", fileName = "LoadingSceneSettings")]
	public class LoadingSceneSettings : ScriptableObject
	{
		[Range(0.1f, 0.9f)]
		public float fakeLoadingRange = 0.2f;
		
		[Range(0.1f, 10f)]
		public float fakeLoadingTime = 10f;

		public AnimationCurve fakeLoadingProgressOverTime = AnimationCurve.Linear(0, 0, 1, 1);
	}

}