using LogService.ClientBase.Models;

namespace LogService.ClientBase.Messages;

public class SaveProfile
{
	public ProfileBase Profile { get; }

	public SaveProfile(ProfileBase profile)
	{
		Profile = profile;
	}
}