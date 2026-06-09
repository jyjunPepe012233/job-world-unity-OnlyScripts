using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{

	[CreateAssetMenu(fileName = "OpenURLModel", menuName = "Jobworld/Model/OpenURL")]
	public class OpenURLModelSO : ScriptableObject, IOpenURLModel
	{
		public string url = "https://www.google.com/";

		public void OpenURL()
		{
			Application.OpenURL(url);
		}
	}

}