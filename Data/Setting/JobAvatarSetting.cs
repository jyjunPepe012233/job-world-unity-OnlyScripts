using UnityEngine;

namespace Jobworld
{
	[CreateAssetMenu(fileName = "JOBNAME_AvatarSetting", menuName = "Jobworld/Setting/Avatar Setting")]
	public class AvatarSetting : ScriptableObject
	{
		public GameObject avatarPrefab;
	}

}