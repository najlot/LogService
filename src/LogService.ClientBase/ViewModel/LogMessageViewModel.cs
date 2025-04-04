﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using LogService.Client.Data.Models;
using LogService.Client.Data.Services;
using LogService.Client.MVVM;
using LogService.Client.MVVM.Services;
using LogService.Client.MVVM.Validation;
using LogService.Client.MVVM.ViewModel;
using LogService.ClientBase.Validation;
using LogService.Contracts;
using LogService.Contracts.Events;

namespace LogService.ClientBase.ViewModel;

public partial class LogMessageViewModel : AbstractValidationViewModel, IDisposable
{
	private bool _isBusy;
	private LogMessageModel _item;

	public List<LogLevel> AvailableLogLevels { get; } = new(Enum.GetValues(typeof(LogLevel)) as LogLevel[]);

	private readonly Func<LogArgumentViewModel> _logArgumentViewModelFactory;
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private readonly ILogMessageService _logMessageService;
	private readonly IMessenger _messenger;
	private readonly IMap _map;

	public LogMessageModel Item
	{
		get => _item;
		set
		{
			Set(nameof(Item), ref _item, value);

			if (Item.Arguments == null)
			{
				Arguments = new();
			}
			else
			{
				Arguments = new(Item.Arguments.Select(e =>
				{
					var model = _map.From(e).To<LogArgumentModel>();
					var viewModel = _logArgumentViewModelFactory();
					viewModel.ParentId = Item.Id;
					viewModel.Item = model;
					return viewModel;
				}));
			}
		}
	}

	public bool IsBusy { get => _isBusy; private set => Set(nameof(IsBusy), ref _isBusy, value); }
	public bool IsNew { get; set; }

	public LogMessageViewModel(
		Func<LogArgumentViewModel> logArgumentViewModelFactory,
		IErrorService errorService,
		INavigationService navigationService,
		ILogMessageService logMessageService,
		IMessenger messenger,
		IMap map)
	{
		_logArgumentViewModelFactory = logArgumentViewModelFactory;

		_errorService = errorService;
		_navigationService = navigationService;
		_logMessageService = logMessageService;
		_messenger = messenger;
		_map = map;

		SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
		DeleteCommand = new AsyncCommand(DeleteAsync, DisplayError);

		SetValidation(new LogMessageValidationList());

		_messenger.Register<LogMessageUpdated>(Handle);
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}

	public void Handle(LogMessageUpdated obj)
	{
		if (Item.Id != obj.Id)
		{
			return;
		}

		Item = _map.From(obj).To<LogMessageModel>();

		Arguments = new(Item.Arguments.Select(e =>
		{
			var model = _map.From(e).To<LogArgumentModel>();
			var viewModel = _logArgumentViewModelFactory();
			viewModel.ParentId = Item.Id;
			viewModel.Item = model;
			return viewModel;
		}));
	}

	public AsyncCommand SaveCommand { get; }
	public async Task SaveAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			Item.Arguments = _map.From(Arguments.Select(e => e.Item)).ToList<LogArgumentModel>();

			var errors = Errors
				.Where(err => err.Severity > ValidationSeverity.Info)
				.Select(e => e.Text);

			if (errors.Any())
			{
				var message = "There are some validation errors:";
				message += Environment.NewLine + Environment.NewLine;
				message += string.Join(Environment.NewLine, errors);
				message += Environment.NewLine + Environment.NewLine;
				message += "Do you want to continue?";

				var vm = new YesNoPageViewModel()
				{
					Title = "Validation",
					Message = message
				};

				var selection = await _navigationService.RequestInputAsync(vm);

				if (!selection)
				{
					return;
				}
			}

			await _navigationService.NavigateBack();

			if (IsNew)
			{
				await _logMessageService.AddItemAsync(Item);
				IsNew = false;
			}
			else
			{
				await _logMessageService.UpdateItemAsync(Item);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error saving...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand DeleteCommand { get; }
	public async Task DeleteAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var vm = new YesNoPageViewModel()
			{
				Title = "Delete?",
				Message = "Should the item be deleted?"
			};

			var selection = await _navigationService.RequestInputAsync(vm);

			if (selection)
			{
				await _navigationService.NavigateBack();
				await _logMessageService.DeleteItemAsync(Item.Id);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error deleting...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public void Dispose() => _messenger.Unregister(this);
}