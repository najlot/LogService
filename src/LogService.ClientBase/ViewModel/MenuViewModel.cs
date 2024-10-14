using System;
using System.Threading.Tasks;
using LogService.Client.MVVM;
using LogService.Client.MVVM.Services;
using LogService.Client.MVVM.ViewModel;

namespace LogService.ClientBase.ViewModel
{
	public class MenuViewModel : AbstractViewModel
	{
		private readonly IErrorService _errorService;
		private readonly INavigationService _navigationService;
		private bool _isBusy = false;

		private readonly AllLogMessagesViewModel _allLogMessagesViewModel;

		public AsyncCommand NavigateToLogMessages { get; }
		public async Task NavigateToLogMessagesAsync()
		{
			if (_isBusy)
			{
				return;
			}

			try
			{
				_isBusy = true;

				var refreshTask = _allLogMessagesViewModel.RefreshLogMessagesAsync();
				await _navigationService.NavigateForward(_allLogMessagesViewModel);
				await refreshTask;
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Could not load...", ex);
			}
			finally
			{
				_isBusy = false;
			}
		}

		public MenuViewModel(IErrorService errorService,
			AllLogMessagesViewModel allLogMessagesViewModel,
			INavigationService navigationService)
		{
			_errorService = errorService;
			_allLogMessagesViewModel = allLogMessagesViewModel;
			_navigationService = navigationService;

			NavigateToLogMessages = new AsyncCommand(NavigateToLogMessagesAsync, DisplayError);
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlertAsync("Error...", task.Exception);
		}
	}
}