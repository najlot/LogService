using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LogService.Client.Data.Mappings;
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

namespace LogService.ClientBase.ViewModel
{
	public class LogArgumentViewModel : AbstractValidationViewModel
	{
		private bool _isBusy;
		private LogArgumentModel _item;
		private Guid _parentId;

		private readonly IErrorService _errorService;
		private readonly INavigationService _navigationService;
		private readonly IMessenger _messenger;

		public LogArgumentModel Item
		{
			get => _item;
			set
			{
				Set(nameof(Item), ref _item, value);
			}
		}

		public bool IsBusy { get => _isBusy; private set => Set(nameof(IsBusy), ref _isBusy, value); }
		public bool IsNew { get; set; }
		public Guid ParentId { get => _parentId; set => Set(nameof(ParentId), ref _parentId, value); }

		public LogArgumentViewModel(
			IErrorService errorService,
			INavigationService navigationService,
			IMessenger messenger)
		{
			_errorService = errorService;
			_navigationService = navigationService;
			_messenger = messenger;

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
				await _messenger.SendAsync(new SaveLogArgument(_parentId, Item));
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

					await _messenger.SendAsync(new DeleteLogArgument(_parentId, Item.Id));
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
				await _messenger.SendAsync(new EditLogArgument(_parentId, Item.Id));
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
}