using Cosei.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using LogService.Contracts;
using LogService.Blazor.Model;
using LogService.Blazor.Repository;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using LogService.Contracts.Filters;

namespace LogService.Blazor.Services;

public class LogMessageService
{
	private readonly ILogMessageRepository _logMessageRepository;
	private readonly IPublisher _publisher;
	private readonly IMap _map;

	public LogMessageService(
		ILogMessageRepository logMessageRepository,
		IPublisher publisher,
		IMap map)
	{
		_logMessageRepository = logMessageRepository;
		_publisher = publisher;
		_map = map;
	}

	public async Task CreateLogMessages(CreateLogMessage[] commands, string source, Guid userId)
	{
		var items = _map.From<CreateLogMessage>(commands).ToArray<LogMessageModel>();

		int argId = 0;

		for (int i = 0; i < items.Length; i++)
		{
			items[i].Id = Guid.NewGuid();
			items[i].CreatedBy = userId;
			items[i].Source = source;

			foreach (var argument in items[i].Arguments)
			{
				argument.Id = argId++;
				argument.Key ??= string.Empty;
				argument.Value ??= string.Empty;
			}
		}

		await _logMessageRepository.Insert(items).ConfigureAwait(false);

		var messages = _map.From<LogMessageModel>(items).ToList<LogMessageCreated>();
		await _publisher.PublishToUserAsync(userId.ToString(), messages).ConfigureAwait(false);
	}

	public async Task<LogMessage?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await _logMessageRepository.Get(id).ConfigureAwait(false);
		return _map.FromNullable(item)?.To<LogMessage>();
	}

	public LogMessageListItem[] GetItemsForUserAsync(LogMessageFilter filter, Guid userId)
	{
		var queryable = _logMessageRepository
			.GetAllQueryable()
			.Where(e => e.CreatedBy == userId);

		if (filter.DateTimeFrom != null)
			queryable = queryable.Where(e => e.DateTime >= filter.DateTimeFrom);
		if (filter.DateTimeTo != null)
			queryable = queryable.Where(e => e.DateTime <= filter.DateTimeTo);
		if (filter.LogLevel != null)
			queryable = queryable.Where(e => e.LogLevel >= filter.LogLevel);
		if (!string.IsNullOrEmpty(filter.Category))
			queryable = queryable.Where(e => e.Category.Contains(filter.Category));
		if (!string.IsNullOrEmpty(filter.State))
			queryable = queryable.Where(e => e.State.Contains(filter.State));
		if (!string.IsNullOrEmpty(filter.Source))
			queryable = queryable.Where(e => e.Source.Contains(filter.Source));
		if (!string.IsNullOrEmpty(filter.RawMessage))
			queryable = queryable.Where(e => e.RawMessage.Contains(filter.RawMessage));
		if (!string.IsNullOrEmpty(filter.Message))
			queryable = queryable.Where(e => e.Message.Contains(filter.Message));
		if (!string.IsNullOrEmpty(filter.Exception))
			queryable = queryable.Where(e => e.Exception.Contains(filter.Exception));
		if (filter.ExceptionIsValid != null)
			queryable = queryable.Where(e => e.ExceptionIsValid == filter.ExceptionIsValid);

		queryable = queryable.OrderByDescending(e => e.DateTime);

		var sourceItems = queryable.ToArray();

		return _map.From<LogMessageModel>(sourceItems).ToArray<LogMessageListItem>();
	}

	public LogMessageListItem[] GetItemsForUserAsync(Guid userId)
	{
		var queryable = _logMessageRepository
			.GetAllQueryable()
			.Where(e => e.CreatedBy == userId)
			.OrderByDescending(e => e.DateTime);

		var sourceItems = queryable.ToArray();

		return _map.From<LogMessageModel>(sourceItems).ToArray<LogMessageListItem>();
	}
}