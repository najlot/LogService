using LogService.ClientBase.Models;

namespace LogService.ClientBase.Messages
{
	public class LoginProfile
	{
		public ProfileBase Profile { get; }

		public LoginProfile(ProfileBase profile)
		{
			Profile = profile;
		}
	}
}