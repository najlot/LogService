using LogService.ClientBase.Models;

namespace LogService.ClientBase.Messages
{
	public class DeleteProfile
	{
		public ProfileBase Profile { get; }

		public DeleteProfile(ProfileBase profile)
		{
			Profile = profile;
		}
	}
}