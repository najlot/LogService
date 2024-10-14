using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LogService.ClientBase.Messages;
using LogService.ClientBase.Validation;
using LogService.Client.Localisation;
using LogService.Client.MVVM;
using LogService.Client.MVVM.ViewModel;
using LogService.Client.MVVM.Services;
using LogService.Client.Data.Services;
using LogService.Client.Data.Mappings;
using LogService.Client.Data.Models;
using LogService.Contracts.Events;

namespace LogService.ClientBase.ViewModel
{
	public class AllLogMessagesViewModel : AbstractViewModel, IDisposable
	{
		private readonly Func<LogMessageViewModel> _logMessageViewModelFactory;
		private readonly ILogMessageService _logMessageService;
		private readonly INavigationService _navigationService;
		private readonly IMessenger _messenger;
		private readonly IErrorService _errorService;

		private bool _isBusy;
		private string _filter;

		public bool IsBusy
		{
			get => _isBusy;
			set => Set(nameof(IsBusy), ref _isBusy, value);
		}

		public string Filter
		{
			get => _filter;
			set
			{
				Set(nameof(Filter), ref _filter, value);
				LogMessagesView.Refresh();
			}
		}

		public ObservableCollectionView<LogMessageListItemModel> LogMessagesView { get; }
		public ObservableCollection<LogMessageListItemModel> LogMessages { get; } = new ObservableCollection<LogMessageListItemModel>();

		public AllLogMessagesViewModel(
			Func<LogMessageViewModel> logMessageViewModelFactory,
			IErrorService errorService,
			ILogMessageService logMessageService,
			INavigationService navigationService,
			IMessenger messenger)
		{
			_logMessageViewModelFactory = logMessageViewModelFactory;
			_errorService = errorService;
			_logMessageService = logMessageService;
			_navigationService = navigationService;
			_messenger = messenger;

			LogMessagesView = new ObservableCollectionView<LogMessageListItemModel>(LogMessages, FilterLogMessage);

			_messenger.Register<LogMessageCreated>(Handle);
			_messenger.Register<LogMessageUpdated>(Handle);
			_messenger.Register<LogMessageDeleted>(Handle);

			AddLogMessageCommand = new AsyncCommand(AddLogMessageAsync, DisplayError);
			EditLogMessageCommand = new AsyncCommand<LogMessageListItemModel>(EditLogMessageAsync, DisplayError);
			RefreshLogMessagesCommand = new AsyncCommand(RefreshLogMessagesAsync, DisplayError);
		}

		private bool FilterLogMessage(LogMessageListItemModel item)
		{
			if (string.IsNullOrEmpty(Filter))
			{
				return true;
			}

			var dateTime = item.DateTime.ToString();
			if (!string.IsNullOrEmpty(dateTime) && dateTime.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}

			var logLevel = item.LogLevel.ToString();
			if (!string.IsNullOrEmpty(logLevel) && logLevel.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return true;
			}

			return false;
		}

		private async Task DisplayError(Task task)
		{
			await _errorService.ShowAlertAsync(CommonLoc.Error, task.Exception);
		}

		private void Handle(LogMessageCreated obj)
		{
			LogMessages.Insert(0, new LogMessageListItemModel()
			{
				Id = obj.Id,
				DateTime = obj.DateTime,
				LogLevel = obj.LogLevel,
			});
		}

		private void Handle(LogMessageUpdated obj)
		{
			var oldItem = LogMessages.FirstOrDefault(i => i.Id == obj.Id);
			var index = -1;

			if (oldItem != null)
			{
				index = LogMessages.IndexOf(oldItem);

				if (index != -1)
				{
					LogMessages.RemoveAt(index);
				}
			}

			if (index == -1)
			{
				index = 0;
			}

			LogMessages.Insert(index, new LogMessageListItemModel()
			{
				Id = obj.Id,
				DateTime = obj.DateTime,
				LogLevel = obj.LogLevel,
			});
		}

		private void Handle(LogMessageDeleted obj)
		{
			var oldItem = LogMessages.FirstOrDefault(i => i.Id == obj.Id);

			if (oldItem != null)
			{
				LogMessages.Remove(oldItem);
			}
		}

		public AsyncCommand<LogMessageListItemModel> EditLogMessageCommand { get; }
		public async Task EditLogMessageAsync(LogMessageListItemModel model)
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var item = await _logMessageService.GetItemAsync(model.Id);
				var viewModel = _logMessageViewModelFactory();


				viewModel.Item = item;

				_messenger.Register<EditLogArgument>(viewModel.Handle);
				_messenger.Register<DeleteLogArgument>(viewModel.Handle);
				_messenger.Register<SaveLogArgument>(viewModel.Handle);

				_messenger.Register<LogMessageUpdated>(viewModel.Handle);

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

		public AsyncCommand AddLogMessageCommand { get; }
		public async Task AddLogMessageAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var item = _logMessageService.CreateLogMessage();
				var viewModel = _logMessageViewModelFactory();


				viewModel.Item = item;
				viewModel.IsNew = true;

				_messenger.Register<EditLogArgument>(viewModel.Handle);
				_messenger.Register<DeleteLogArgument>(viewModel.Handle);
				_messenger.Register<SaveLogArgument>(viewModel.Handle);

				await _navigationService.NavigateForward(viewModel);
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error adding...", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public AsyncCommand RefreshLogMessagesCommand { get; }
		public async Task RefreshLogMessagesAsync()
		{
			if (IsBusy)
			{
				return;
			}

			try
			{
				IsBusy = true;
				LogMessagesView.Disable();
				Filter = "";

				LogMessages.Clear();

				var logMessages = await _logMessageService.GetItemsAsync(true);

				foreach (var item in logMessages)
				{
					LogMessages.Add(item);
				}
			}
			catch (Exception ex)
			{
				await _errorService.ShowAlertAsync("Error loading data...", ex);
			}
			finally
			{
				LogMessagesView.Enable();
				IsBusy = false;
			}
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					_messenger.Unregister(this);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}