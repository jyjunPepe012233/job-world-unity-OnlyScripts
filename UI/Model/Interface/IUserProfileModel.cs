using System.Threading.Tasks;

namespace Jobworld
{

	public interface IUserProfileModel
	{
		Task<UserProfile> GetUserProfileDataById(long id);
	}

}