using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.MVVM;
using LogService.Client.MVVM.Services;
using LogService.Client.MVVM.ViewModel;
using LogService.ClientBase.Validation;

namespace LogService.ClientBase.ViewModel;

public class LogArgumentViewModel : AbstractValidationViewModel
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;

	private int _id;
	public int Id { get => _id; set => Set(ref _id, value); }

	private string _key = string.Empty;
	public string Key { get => _key; set => Set(ref _key, value); }

	private string _value = string.Empty;
	public string Value { get => _value; set => Set(ref _value, value); }

	private Guid _parentId;
	public Guid ParentId { get => _parentId; set => Set(nameof(ParentId), ref _parentId, value); }

	public LogArgumentViewModel(
		IErrorService errorService,
		INavigationService navigationService)
	{
		_errorService = errorService;
		_navigationService = navigationService;

		SaveCommand = new AsyncCommand(RequestSave, DisplayError);
		DeleteCommand = new AsyncCommand(RequestDelete, DisplayError);

		SetValidation(new LogArgumentValidationList());
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}

	private readonly List<Func<LogArgumentViewModel, Task>> _onSaveRequested = [];
	public void OnSaveRequested(Func<LogArgumentViewModel, Task> func) => _onSaveRequested.Add(func);

	public AsyncCommand SaveCommand { get; }
	private async Task RequestSave()
	{
		foreach (var func in _onSaveRequested)
		{
			await func(this);
		}
	}

	private readonly List<Func<LogArgumentViewModel, Task<bool>>> _onDeleteRequested = [];
	public void OnDeleteRequested(Func<LogArgumentViewModel, Task<bool>> func) => _onDeleteRequested.Add(func);

	public AsyncCommand DeleteCommand { get; }
	private async Task RequestDelete()
	{
		bool navigateBack = true;

		foreach (var func in _onDeleteRequested)
		{
			if (!await func(this))
			{
				navigateBack = false;
			}
		}

		if (navigateBack)
		{
			await _navigationService.NavigateBack();
		}
	}
}