using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.MVVM;
using LogService.ClientBase.Messages;
using LogService.ClientBase.Models;
using LogService.ClientBase.Services;
using LogService.ClientBase.Validation;
using LogService.Contracts;

namespace LogService.ClientBase.ViewModel;

public partial class LogMessageViewModel
{
	private ObservableCollection<LogArgumentViewModel> _arguments = new ObservableCollection<LogArgumentViewModel>();
	public ObservableCollection<LogArgumentViewModel> Arguments { get => _arguments; set => Set(nameof(Arguments), ref _arguments, value); }

	public RelayCommand AddLogArgumentCommand => new RelayCommand(() =>
	{
		var max = 0;

		if (Arguments.Count > 0)
		{
			max = Arguments.Max(e => e.Item.Id) + 1;
		}

		var model = new LogArgumentModel() { Id = max };

		var viewModel = _logArgumentViewModelFactory();
		viewModel.ParentId = Item.Id;
		viewModel.Item = model;

		Arguments.Add(viewModel);
	});

	public async Task Handle(DeleteLogArgument obj)
	{
		if (Item.Id != obj.ParentId)
		{
			return;
		}

		try
		{
			var oldItem = Arguments.FirstOrDefault(i => i.Item.Id == obj.Id);

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

		if (Item.Id != obj.ParentId)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var vm = Arguments.FirstOrDefault(e => e.Item.Id == obj.Id);
			var viewModel = _logArgumentViewModelFactory();
			viewModel.ParentId = Item.Id;
			viewModel.Item = vm.Item;

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
		if (Item.Id != obj.ParentId)
		{
			return;
		}

		try
		{
			int index = -1;
			var oldItem = Arguments.FirstOrDefault(i => i.Item.Id == obj.Item.Id);

			if (oldItem != null)
			{
				index = Arguments.IndexOf(oldItem);

				if (index != -1)
				{
					Arguments.RemoveAt(index);
				}
			}

			var viewModel = _logArgumentViewModelFactory();
			viewModel.ParentId = Item.Id;
			viewModel.Item = obj.Item;

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