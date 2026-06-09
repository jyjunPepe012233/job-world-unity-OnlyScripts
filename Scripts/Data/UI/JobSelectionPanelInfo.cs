using UnityEngine;

namespace Jobworld
{

	[CreateAssetMenu(fileName = "JOBNAME_JobSelectionPanelInfo", menuName = "Jobworld/UI/Job Selection Panel Info")]
	public class JobSelectionPanelInfo : ScriptableObject
	{
		public string jobName;
		
		[TextArea(8, 8)]
		public string description;

		public Sprite banner;

		// 튜토리얼 씬 이름을 저장하는 필드 추가
		public string tutorialSceneName;
	}

}