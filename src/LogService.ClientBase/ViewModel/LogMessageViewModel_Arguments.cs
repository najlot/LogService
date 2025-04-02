using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.MVVM;
using LogService.Client.MVVM.Services;
using LogService.Client.MVVM.ViewModel;

namespace LogService.ClientBase.ViewModel;

public partial class LogMessageViewModel
{
	private ObservableCollection<LogArgumentViewModel> _arguments = [];
	public ObservableCollection<LogArgumentViewModel> Arguments { get => _arguments; set => Set(nameof(Arguments), ref _arguments, value); }

	public RelayCommand AddLogArgumentCommand => new(AddLogArgument);
	private void AddLogArgument()
	{
		var max = 0;

		if (Arguments.Count > 0)
		{
			max = Arguments.Max(e => e.Id) + 1;
		}

		var model = new LogArgumentModel() { Id = max };
		var viewModel = _map.From(model).To<LogArgumentViewModel>();
		viewModel.Id = max;
		viewModel.ParentId = Id;

		Arguments.Add(viewModel);
	}

	public AsyncCommand<LogArgumentViewModel> EditLogArgumentCommand => new(EditLogArgument, DisplayError);
	private async Task EditLogArgument(LogArgumentViewModel vm)
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var viewModel = _map.From(vm).To<LogArgumentViewModel>();
			viewModel.ParentId = Id;

			viewModel.OnSaveRequested(SaveLogArgumentAsync);
			viewModel.OnDeleteRequested(DeleteLogArgumentAsync);

			await _navigationService.NavigateForward(viewModel);
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error loading...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task SaveLogArgumentAsync(LogArgumentViewModel viewModel)
	{
		try
		{
			var vm = Arguments.FirstOrDefault(i => i.Id == viewModel.Id);
			_map.From(viewModel).ToNullable(vm);

			await _navigationService.NavigateBack();
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error saving...", ex);
		}
	}

	public AsyncCommand<LogArgumentViewModel> DeleteLogArgumentCommand => new(DeleteLogArgumentAsync, DisplayError);
	private async Task<bool> DeleteLogArgumentAsync(LogArgumentViewModel viewModel)
	{
		if (IsBusy)
		{
			return false;
		}

		try
		{
			IsBusy = true;

			var yesNoPageViewModel = new YesNoPageViewModel()
			{
				Title = "Delete?",
				Message = "Should the item be deleted?"
			};

			var selection = await _navigationService.RequestInputAsync(yesNoPageViewModel);

			if (selection)
			{
				var oldItem = Arguments.FirstOrDefault(i => i.Id == viewModel.Id);

				if (oldItem != null)
				{
					var index = Arguments.IndexOf(oldItem);

					if (index != -1)
					{
						Arguments.RemoveAt(index);
					}
				}
			}

			return selection;
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error deleting...", ex);
		}
		finally
		{
			IsBusy = false;
		}

		return false;
	}
}