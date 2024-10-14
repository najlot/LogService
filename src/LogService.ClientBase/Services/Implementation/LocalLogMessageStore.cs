using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LogService.Client.Data.Mappings;
using LogService.Client.Data.Models;
using LogService.Client.Data.Repositories;
using LogService.ClientBase.ProfileHandler;
using LogService.Contracts;
using LogService.Contracts.Events;
using LogService.Contracts.Filters;

namespace LogService.ClientBase.Services.Implementation
{
	public sealed class LocalLogMessageStore : ILogMessageRepository
	{
		private readonly string _dataPath;
		private readonly ILocalSubscriber _subscriber;
		private List<LogMessageModel> _items = null;

		public LocalLogMessageStore(string folderName, ILocalSubscriber localSubscriber)
		{
			var appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LogService");
			appdataDir = Path.Combine(appdataDir, folderName);
			Directory.CreateDirectory(appdataDir);

			_dataPath = Path.Combine(appdataDir, "LogMessages.json");
			_items = GetItems();
			_subscriber = localSubscriber;
		}

		private List<LogMessageModel> GetItems()
		{
			List<LogMessageModel> items;
			if (File.Exists(_dataPath))
			{
				var data = File.ReadAllText(_dataPath);
				items = JsonSerializer.Deserialize<List<LogMessageModel>>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
			}
			else
			{
				items = new List<LogMessageModel>();
			}

			return items;
		}

		public async Task<bool> AddItemAsync(LogMessageModel item)
		{
			var logArgumentMapper = new LogArgumentMapper();
			_items.Insert(0, item);

			SaveItems();

			await _subscriber.SendAsync(new LogMessageCreated(
				item.Id,
				item.DateTime,
				item.LogLevel,
				item.Category,
				item.State,
				item.Source,
				item.RawMessage,
				item.Message,
				item.Exception,
				item.ExceptionIsValid,
				item.RawArguments,
				item.Arguments.Select(e => logArgumentMapper.Map(e, new LogArgument())).ToList()));

			return await Task.FromResult(true);
		}

		private void SaveItems()
		{
			var text = JsonSerializer.Serialize(_items);
			File.WriteAllText(_dataPath, text);
		}

		public async Task<bool> UpdateItemAsync(LogMessageModel item)
		{
			var logArgumentMapper = new LogArgumentMapper();
			int index = 0;
			var oldItem = _items.FirstOrDefault(i => i.Id == item.Id);

			if (oldItem != null)
			{
				index = _items.IndexOf(oldItem);

				if (index != -1)
				{
					_items.RemoveAt(index);
				}
				else
				{
					index = 0;
				}
			}

			_items.Insert(index, item);

			SaveItems();

			await _subscriber.SendAsync(new LogMessageUpdated(
				item.Id,
				item.DateTime,
				item.LogLevel,
				item.Category,
				item.State,
				item.Source,
				item.RawMessage,
				item.Message,
				item.Exception,
				item.ExceptionIsValid,
				item.RawArguments,
				item.Arguments.Select(e => logArgumentMapper.Map(e, new LogArgument())).ToList()));

			return await Task.FromResult(true);
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			var oldItem = _items.FirstOrDefault(arg => arg.Id == id);
			_items.Remove(oldItem);

			SaveItems();

			await _subscriber.SendAsync(new LogMessageDeleted(id));

			return await Task.FromResult(true);
		}

		public async Task<LogMessageModel> GetItemAsync(Guid id)
		{
			return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
		}

		public async Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(bool forceRefresh = false)
		{
			_items = GetItems();

			return await Task.FromResult(_items.Select(e => new LogMessageListItemModel()
			{
				Id = e.Id,
				DateTime = e.DateTime,
				LogLevel = e.LogLevel,
			}));
		}

		public async Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(LogMessageFilter filter)
		{
			IEnumerable<LogMessageModel> enumerable = GetItems();

			if (filter.DateTimeFrom != null)
				enumerable = enumerable.Where(e => e.DateTime >= filter.DateTimeFrom);
			if (filter.DateTimeTo != null)
				enumerable = enumerable.Where(e => e.DateTime <= filter.DateTimeTo);
			if (filter.LogLevel != null)
				enumerable = enumerable.Where(e => e.LogLevel == filter.LogLevel);
			if (filter.Category != null)
				enumerable = enumerable.Where(e => e.Category.ToLower().Contains(filter.Category.ToLower()));
			if (filter.State != null)
				enumerable = enumerable.Where(e => e.State.ToLower().Contains(filter.State.ToLower()));
			if (filter.Source != null)
				enumerable = enumerable.Where(e => e.Source.ToLower().Contains(filter.Source.ToLower()));
			if (filter.RawMessage != null)
				enumerable = enumerable.Where(e => e.RawMessage.ToLower().Contains(filter.RawMessage.ToLower()));
			if (filter.Message != null)
				enumerable = enumerable.Where(e => e.Message.ToLower().Contains(filter.Message.ToLower()));
			if (filter.Exception != null)
				enumerable = enumerable.Where(e => e.Exception.ToLower().Contains(filter.Exception.ToLower()));
			if (filter.ExceptionIsValid != null)
				enumerable = enumerable.Where(e => e.ExceptionIsValid == filter.ExceptionIsValid);
			if (filter.RawArguments != null)
				enumerable = enumerable.Where(e => e.RawArguments == filter.RawArguments);

			return await Task.FromResult(enumerable.Select(e => new LogMessageListItemModel()
			{
				Id = e.Id,
				DateTime = e.DateTime,
				LogLevel = e.LogLevel,
			}));
		}

		public void Dispose()
		{
			// Nothing to do
		}
	}
}