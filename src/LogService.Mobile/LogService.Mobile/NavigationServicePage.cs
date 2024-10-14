using System.Threading.Tasks;
using LogService.Client.MVVM.Services;
using LogService.Client.MVVM.ViewModel;
using LogService.ClientBase.ViewModel;
using LogService.Mobile.View;
using Xamarin.Forms;

namespace LogService.Mobile
{
	public class NavigationServicePage : NavigationPage, INavigationService
	{
		public NavigationServicePage(Page root) : base(root)
		{
		}

		public async Task NavigateBack()
		{
			if (Navigation.ModalStack.Count > 0)
			{
				await Navigation.PopModalAsync();
			}
			else
			{
				await Navigation.PopAsync();
			}
		}

		public async Task NavigateForward(AbstractViewModel vm)
		{
			ContentPage cp = null;
			bool isPopup = vm is IPopupViewModel;

			if (vm is LoginViewModel)
			{
				cp = new LoginView();
			}
			else if (vm is AllLogMessagesViewModel)
			{
				cp = new AllLogMessagesView();
			}
			else if (vm is YesNoPageViewModel)
			{
				cp = new YesNoPageView();
			}
			else if (vm is LogArgumentViewModel)
			{
				cp = new LogArgumentView();
			}
			else if (vm is LogMessageViewModel)
			{
				cp = new LogMessageView();
			}
			else if (vm is MenuViewModel)
			{
				cp = new MenuView();
			}
			else if (vm is AlertViewModel)
			{
				cp = new AlertView();
			}
			else if (vm is ProfileViewModel)
			{
				cp = new ProfileView();
			}

			cp.BindingContext = vm;

			await Task.Delay(100);

			if (isPopup)
			{
				SetHasBackButton(cp, false);
				await Navigation.PushModalAsync(cp);
			}
			else
			{
				await Navigation.PushAsync(cp);
			}
		}
	}
}