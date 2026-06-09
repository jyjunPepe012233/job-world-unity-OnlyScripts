using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{
	
	[CreateAssetMenu(fileName = "MyDataModel", menuName = "Jobworld/Model/MyData")]
	public class MyDataModelSO : ScriptableObject, IMyDataModel
	{
		public async Task<UserData> GetMyData()
		{
			var response = await DataInterfaceContainer.user.GetMyData();
			if (response.GetStatus() == 200)
			{
				return response.Child("data").Parse<UserData>();
			}
			return new UserData();
		}
	}

}