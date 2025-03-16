using Cosei.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using LogService.Contracts;
using LogService.Service.Model;
using LogService.Service.Repository;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using LogService.Contracts.Filters;

namespace LogService.Service.Services;

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

	public async Task CreateLogMessage(CreateLogMessage command, Guid userId)
	{
		var item = _map.From(command).To<LogMessageModel>();

		await _logMessageRepository.Insert(item).ConfigureAwait(false);

		var message = _map.From(item).To<LogMessageCreated>();
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task UpdateLogMessage(UpdateLogMessage command, Guid userId)
	{
		var item = await _logMessageRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("LogMessage not found!");
		}

		_map.From(command).To(item);

		await _logMessageRepository.Update(item).ConfigureAwait(false);

		var message = _map.From(item).To<LogMessageUpdated>();
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task DeleteLogMessage(Guid id, Guid userId)
	{
		await _logMessageRepository.Delete(id).ConfigureAwait(false);

		var message = new LogMessageDeleted(id);
		await _publisher.PublishAsync(message).ConfigureAwait(false);
	}

	public async Task<LogMessage?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await _logMessageRepository.Get(id).ConfigureAwait(false);
		return _map.FromNullable(item)?.To<LogMessage>();
	}

	public IAsyncEnumerable<LogMessageListItem> GetItemsForUserAsync(LogMessageFilter filter, Guid userId)
	{
		var enumerable = _logMessageRepository.GetAll();

		if (filter.DateTimeFrom != null)
			enumerable = enumerable.Where(e => e.DateTime >= filter.DateTimeFrom);
		if (filter.DateTimeTo != null)
			enumerable = enumerable.Where(e => e.DateTime <= filter.DateTimeTo);
		if (filter.LogLevel != null)
			enumerable = enumerable.Where(e => e.LogLevel == filter.LogLevel);
		if (!string.IsNullOrEmpty(filter.Category))
			enumerable = enumerable.Where(e => e.Category.Contains(filter.Category));
		if (!string.IsNullOrEmpty(filter.State))
			enumerable = enumerable.Where(e => e.State.Contains(filter.State));
		if (!string.IsNullOrEmpty(filter.Source))
			enumerable = enumerable.Where(e => e.Source.Contains(filter.Source));
		if (!string.IsNullOrEmpty(filter.RawMessage))
			enumerable = enumerable.Where(e => e.RawMessage.Contains(filter.RawMessage));
		if (!string.IsNullOrEmpty(filter.Message))
			enumerable = enumerable.Where(e => e.Message.Contains(filter.Message));
		if (!string.IsNullOrEmpty(filter.Exception))
			enumerable = enumerable.Where(e => e.Exception.Contains(filter.Exception));
		if (filter.ExceptionIsValid != null)
			enumerable = enumerable.Where(e => e.ExceptionIsValid == filter.ExceptionIsValid);

		return _map.From(enumerable).To<LogMessageListItem>();
	}

	public IAsyncEnumerable<LogMessageListItem> GetItemsForUserAsync(Guid userId)
	{
		var enumerable = _logMessageRepository.GetAll();
		return _map.From(enumerable).To<LogMessageListItem>();
	}
}