using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Najlot.Map;
using LogService.Client.Data.Models;
using LogService.Client.Data.Repositories;
using LogService.ClientBase.ProfileHandler;
using LogService.Contracts;
using LogService.Contracts.Events;
using LogService.Contracts.Filters;

namespace LogService.ClientBase.Services.Implementation;

public sealed class LocalLogMessageStore : ILogMessageRepository
{
	private readonly string _dataPath;
	private readonly ILocalSubscriber _subscriber;
	private readonly IMap _map;
	private List<LogMessageModel> _items = null;

	public LocalLogMessageStore(string folderName, ILocalSubscriber localSubscriber, IMap map)
	{
		_map = map;

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

	public async Task AddItemAsync(LogMessageModel item)
	{
		_items.Insert(0, item);

		SaveItems();

		await _subscriber.SendAsync(_map.From(item).To<LogMessageCreated>());
	}

	private void SaveItems()
	{
		var text = JsonSerializer.Serialize(_items);
		File.WriteAllText(_dataPath, text);
	}

	public async Task UpdateItemAsync(LogMessageModel item)
	{
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

		await _subscriber.SendAsync(_map.From(item).To<LogMessageUpdated>());
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var oldItem = _items.FirstOrDefault(arg => arg.Id == id);
		_items.Remove(oldItem);

		SaveItems();

		await _subscriber.SendAsync(new LogMessageDeleted(id));
	}

	public async Task<LogMessageModel> GetItemAsync(Guid id)
	{
		return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
	}

	public Task<LogMessageListItemModel[]> GetItemsAsync()
	{
		_items = GetItems();

		var listItems = _map.From<LogMessageModel>(_items).ToArray<LogMessageListItemModel>();
		return Task.FromResult(listItems);
	}

	public Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter)
	{
		IEnumerable<LogMessageModel> enumerable = GetItems();

		if (filter.DateTimeFrom != null)
			enumerable = enumerable.Where(e => e.DateTime >= filter.DateTimeFrom);
		if (filter.DateTimeTo != null)
			enumerable = enumerable.Where(e => e.DateTime <= filter.DateTimeTo);
		if (filter.LogLevel != null)
			enumerable = enumerable.Where(e => e.LogLevel == filter.LogLevel);
		if (!string.IsNullOrEmpty(filter.Category))
			enumerable = enumerable.Where(e => e.Category.ToLower().Contains(filter.Category.ToLower()));
		if (!string.IsNullOrEmpty(filter.State))
			enumerable = enumerable.Where(e => e.State.ToLower().Contains(filter.State.ToLower()));
		if (!string.IsNullOrEmpty(filter.Source))
			enumerable = enumerable.Where(e => e.Source.ToLower().Contains(filter.Source.ToLower()));
		if (!string.IsNullOrEmpty(filter.RawMessage))
			enumerable = enumerable.Where(e => e.RawMessage.ToLower().Contains(filter.RawMessage.ToLower()));
		if (!string.IsNullOrEmpty(filter.Message))
			enumerable = enumerable.Where(e => e.Message.ToLower().Contains(filter.Message.ToLower()));
		if (!string.IsNullOrEmpty(filter.Exception))
			enumerable = enumerable.Where(e => e.Exception.ToLower().Contains(filter.Exception.ToLower()));
		if (filter.ExceptionIsValid != null)
			enumerable = enumerable.Where(e => e.ExceptionIsValid == filter.ExceptionIsValid);

		var listItems = _map.From<LogMessageModel>(enumerable).ToArray<LogMessageListItemModel>();
		return Task.FromResult(listItems);
	}

	public void Dispose()
	{
		// Nothing to do
	}
}