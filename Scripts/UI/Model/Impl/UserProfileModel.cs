using System.Threading.Tasks;
using UnityEngine;

namespace Jobworld
{

	public class UserProfileModel : IUserProfileModel
	{
		private IUserDataInterface m_userDI = DataInterfaceContainer.user;
		
		public async Task<UserProfile> GetUserProfileDataById(long id)
		{
			var response = await m_userDI.GetUserDataById(id);
			return response.Child("data").Parse<UserProfile>();
		}
	}

}