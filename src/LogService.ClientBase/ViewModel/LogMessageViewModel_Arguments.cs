using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.MVVM;
using LogService.ClientBase.Messages;

namespace LogService.ClientBase.ViewModel;

public partial class LogMessageViewModel
{
	private ObservableCollection<LogArgumentViewModel> _arguments = [];
	public ObservableCollection<LogArgumentViewModel> Arguments { get => _arguments; set => Set(nameof(Arguments), ref _arguments, value); }

	public RelayCommand AddLogArgumentCommand => new(() =>
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
	});

	public async Task Handle(DeleteLogArgument obj)
	{
		if (Id != obj.ParentId)
		{
			return;
		}

		try
		{
			var oldItem = Arguments.FirstOrDefault(i => i.Id == obj.Id);

			if (oldItem != null)
			{
				var index = Arguments.IndexOf(oldItem);

				if (index != -1)
				{
					Arguments.RemoveAt(index);
				}
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error saving...", ex);
		}
	}

	public async Task Handle(EditLogArgument obj)
	{
		if (IsBusy)
		{
			return;
		}

		if (Id != obj.ParentId)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var vm = Arguments.FirstOrDefault(e => e.Id == obj.Id);
			var viewModel = _map.From(vm).To<LogArgumentViewModel>();
			viewModel.ParentId = Id;

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

	public async Task Handle(SaveLogArgument obj)
	{
		if (Id != obj.ParentId)
		{
			return;
		}

		try
		{
			int index = -1;
			var oldItem = Arguments.FirstOrDefault(i => i.Id == obj.Item.Id);

			if (oldItem != null)
			{
				index = Arguments.IndexOf(oldItem);

				if (index != -1)
				{
					Arguments.RemoveAt(index);
				}
			}

			var viewModel = _map.From(obj).To<LogArgumentViewModel>();
			viewModel.ParentId = Id;

			if (index == -1)
			{
				Arguments.Insert(0, viewModel);
			}
			else
			{
				Arguments.Insert(index, viewModel);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error saving...", ex);
		}
	}
}