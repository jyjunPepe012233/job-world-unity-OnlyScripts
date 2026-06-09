using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class TmpTargetLoadSceneNameVisual : MonoBehaviour, ITargetLoadSceneNameVisual
	{
		public TextMeshProUGUI tmp;
		
		public void SetTargetSceneName(string targetSceneName)
		{
			tmp.text = targetSceneName;
		}
	}

}