using UnityEngine;
using UnityEngine.Serialization;

namespace Jobworld
{

	[CreateAssetMenu(fileName = "JOBNAME_Job", menuName = "Jobworld/Job")]
	public class JobSO : ScriptableObject
	{
		public string jobName;
		public Sprite icon;
		
		public JobSelectionPanelInfo jobSelectionPanelInfo; 
		public AvatarSetting avatarSetting;
		
		public TutorialItemSO[] tutorialItemSettings;
	}
}