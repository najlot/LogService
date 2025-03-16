using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using LogService.Client.Data.Models;
using LogService.Client.Data.Services;
using LogService.Client.MVVM;
using LogService.Client.MVVM.Services;
using LogService.Client.MVVM.Validation;
using LogService.Client.MVVM.ViewModel;
using LogService.ClientBase.Messages;
using LogService.ClientBase.Validation;
using LogService.Contracts;
using LogService.Contracts.Events;

namespace LogService.ClientBase.ViewModel;

public partial class LogMessageViewModel : AbstractValidationViewModel
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private readonly ILogMessageService _logMessageService;
	private readonly IMessenger _messenger;
	private readonly IMap _map;

	public List<LogLevel> AvailableLogLevels { get; } = new(Enum.GetValues(typeof(LogLevel)) as LogLevel[]);

	private Guid _id;
	public Guid Id { get => _id; set => Set(ref _id, value); }

	private DateTime _dateTime;
	public DateTime DateTime { get => _dateTime; set => Set(ref _dateTime, value); }

	private LogLevel _logLevel;
	public LogLevel LogLevel { get => _logLevel; set => Set(ref _logLevel, value); }

	private string _category = string.Empty;
	public string Category { get => _category; set => Set(ref _category, value); }

	private string _state = string.Empty;
	public string State { get => _state; set => Set(ref _state, value); }

	private string _source = string.Empty;
	public string Source { get => _source; set => Set(ref _source, value); }

	private string _rawMessage = string.Empty;
	public string RawMessage { get => _rawMessage; set => Set(ref _rawMessage, value); }

	private string _message = string.Empty;
	public string Message { get => _message; set => Set(ref _message, value); }

	private string _exception = string.Empty;
	public string Exception { get => _exception; set => Set(ref _exception, value); }

	private bool _exceptionIsValid;
	public bool ExceptionIsValid { get => _exceptionIsValid; set => Set(ref _exceptionIsValid, value); }

	private bool _isBusy;
	public bool IsBusy { get => _isBusy; private set => Set(ref _isBusy, value); }

	public bool IsNew { get; set; }

	public LogMessageViewModel(
		IErrorService errorService,
		INavigationService navigationService,
		ILogMessageService logMessageService,
		IMessenger messenger,
		IMap map)
	{
		_errorService = errorService;
		_navigationService = navigationService;
		_logMessageService = logMessageService;
		_messenger = messenger;
		_map = map;

		SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
		DeleteCommand = new AsyncCommand(DeleteAsync, DisplayError);

		SetValidation(new LogMessageValidationList());
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}

	public void Handle(LogMessageUpdated obj)
	{
		if (Id != obj.Id)
		{
			return;
		}

		_map.From(obj).To(this);
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

			var model = _map.From(this).To<LogMessageModel>();

			if (IsNew)
			{
				await _logMessageService.AddItemAsync(model);
				IsNew = false;
			}
			else
			{
				await _logMessageService.UpdateItemAsync(model);
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
				await _logMessageService.DeleteItemAsync(Id);
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
}