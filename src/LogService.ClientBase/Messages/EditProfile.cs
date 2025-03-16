using LogService.ClientBase.Models;

namespace LogService.ClientBase.Messages;

public class EditProfile
{
	public ProfileBase Profile { get; }

	public EditProfile(ProfileBase profile)
	{
		Profile = profile;
	}
}