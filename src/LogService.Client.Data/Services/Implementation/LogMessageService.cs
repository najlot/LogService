using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.Data.Repositories;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Services.Implementation
{
	public class LogMessageService : ILogMessageService
	{
		private ILogMessageRepository _store;

		public LogMessageService(ILogMessageRepository dataStore)
		{
			_store = dataStore;
		}

		public LogMessageModel CreateLogMessage()
		{
			return new LogMessageModel()
			{
				Id = Guid.NewGuid(),
				Category = "",
				State = "",
				Source = "",
				RawMessage = "",
				Message = "",
				Exception = "",
				Arguments = new ()
			};
		}

		public async Task<bool> AddItemAsync(LogMessageModel item)
		{
			return await _store.AddItemAsync(item);
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			return await _store.DeleteItemAsync(id);
		}

		public async Task<LogMessageModel> GetItemAsync(Guid id)
		{
			return await _store.GetItemAsync(id);
		}

		public async Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(bool forceRefresh = false)
		{
			return await _store.GetItemsAsync(forceRefresh);
		}

		public async Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(LogMessageFilter filter)
		{
			return await _store.GetItemsAsync(filter);
		}

		public async Task<bool> UpdateItemAsync(LogMessageModel item)
		{
			return await _store.UpdateItemAsync(item);
		}

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				_disposedValue = true;

				if (disposing)
				{
					_store.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}