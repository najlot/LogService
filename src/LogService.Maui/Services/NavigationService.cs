using LogService.Client.MVVM.Services;
using LogService.Client.MVVM.ViewModel;
using LogService.ClientBase.ViewModel;
using LogService.Maui.View;

namespace LogService.Maui.Services;

public class NavigationService : INavigationService
{
	private readonly INavigation _navigation;

	public NavigationService(INavigation navigation)
	{
		_navigation = navigation;
	}

	private static Page GetPage(AbstractViewModel vm) => vm switch
	{
		LoginViewModel => new LoginView(),
		AllLogMessagesViewModel => new AllLogMessagesView(),
		YesNoPageViewModel => new YesNoPageView(),
		LogArgumentViewModel => new LogArgumentView(),
		LogMessageViewModel => new LogMessageView(),
		MenuViewModel => new MenuView(),
		AlertViewModel => new AlertView(),
		ProfileViewModel => new ProfileView(),
		_ => throw new InvalidNavigationException(),
	};

	public async Task NavigateForward(AbstractViewModel vm)
	{
		bool isPopup = vm is IPopupViewModel;
		Page cp = GetPage(vm);
		cp.BindingContext = vm;

		await Task.Delay(100);

		if (isPopup)
		{
			NavigationPage.SetHasBackButton(cp, false);
			await _navigation.PushModalAsync(cp);
		}
		else
		{
			await _navigation.PushAsync(cp);
		}
	}

	public async Task NavigateBack()
	{
		if (_navigation.ModalStack.Count > 0)
		{
			await _navigation.PopModalAsync();
		}
		else
		{
			await _navigation.PopAsync();
		}
	}
}
