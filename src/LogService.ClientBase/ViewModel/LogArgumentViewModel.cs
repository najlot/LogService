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

public class LogArgumentViewModel : AbstractValidationViewModel
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private readonly IMessenger _messenger;
	private readonly IMap _map;


	private int _id;
	public int Id { get => _id; set => Set(ref _id, value); }

	private string _key = string.Empty;
	public string Key { get => _key; set => Set(ref _key, value); }

	private string _value = string.Empty;
	public string Value { get => _value; set => Set(ref _value, value); }

	private bool _isBusy;
	public bool IsBusy { get => _isBusy; private set => Set(ref _isBusy, value); }

	public bool IsNew { get; set; }
	private Guid _parentId;
	public Guid ParentId { get => _parentId; set => Set(nameof(ParentId), ref _parentId, value); }

	public LogArgumentViewModel(
		IErrorService errorService,
		INavigationService navigationService,
		IMessenger messenger,
		IMap map)
	{
		_errorService = errorService;
		_navigationService = navigationService;
		_messenger = messenger;
		_map = map;

		SaveCommand = new AsyncCommand(SaveAsync, DisplayError);
		DeleteCommand = new AsyncCommand<bool>(DeleteAsync, DisplayError);
		EditLogArgumentCommand = new AsyncCommand(EditLogArgumentAsync, DisplayError, () => !IsBusy);

		SetValidation(new LogArgumentValidationList());
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
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

			var model = _map.From(this).To<LogArgumentModel>();
			await _messenger.SendAsync(new SaveLogArgument(_parentId, model));

			IsNew = false;
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

	public AsyncCommand<bool> DeleteCommand { get; }
	public async Task DeleteAsync(bool navBack)
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
				if (navBack)
				{
					await _navigationService.NavigateBack();
				}

				await _messenger.SendAsync(new DeleteLogArgument(_parentId, Id));
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

	public AsyncCommand EditLogArgumentCommand { get; }
	public async Task EditLogArgumentAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await _messenger.SendAsync(new EditLogArgument(_parentId, Id));
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error starting edit...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}
}