using LogService.ClientBase.Models;
using LogService.Client.MVVM.ViewModel;

namespace LogService.ClientBase.ViewModel;

public class LoginProfileViewModel() : AbstractViewModel
{
	private ProfileBase _profile;
	public ProfileBase Profile
	{
		get => _profile;
		set => Set(ref _profile, value);
	}
}