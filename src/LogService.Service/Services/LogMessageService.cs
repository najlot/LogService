﻿using Cosei.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

	public LogMessageService(
		ILogMessageRepository logMessageRepository,
		IPublisher publisher)
	{
		_logMessageRepository = logMessageRepository;
		_publisher = publisher;
	}

	public async Task CreateLogMessage(CreateLogMessage command, Guid userId)
	{
		var item = new LogMessageModel()
		{
			Id = command.Id,
			DateTime = command.DateTime,
			LogLevel = command.LogLevel,
			Category = command.Category,
			State = command.State,
			Source = command.Source,
			RawMessage = command.RawMessage,
			Message = command.Message,
			Exception = command.Exception,
			ExceptionIsValid = command.ExceptionIsValid,
			Arguments = command.Arguments,
		};

		await _logMessageRepository.Insert(item).ConfigureAwait(false);

		await _publisher.PublishAsync(new LogMessageCreated(
			command.Id,
			command.DateTime,
			command.LogLevel,
			command.Category,
			command.State,
			command.Source,
			command.RawMessage,
			command.Message,
			command.Exception,
			command.ExceptionIsValid,
			command.Arguments)).ConfigureAwait(false);
	}

	public async Task UpdateLogMessage(UpdateLogMessage command, Guid userId)
	{
		var item = await _logMessageRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("LogMessage not found!");
		}

		item.DateTime = command.DateTime;
		item.LogLevel = command.LogLevel;
		item.Category = command.Category;
		item.State = command.State;
		item.Source = command.Source;
		item.RawMessage = command.RawMessage;
		item.Message = command.Message;
		item.Exception = command.Exception;
		item.ExceptionIsValid = command.ExceptionIsValid;

		while (item.Arguments.Count > command.Arguments.Count)
		{
			item.Arguments.RemoveAt(item.Arguments.Count - 1);
		}

		while (item.Arguments.Count < command.Arguments.Count)
		{
			item.Arguments.Add(new LogArgument());
		}

		for (int i = 0; i < item.Arguments.Count; i++)
		{
			item.Arguments[i].Key = command.Arguments[i].Key;
			item.Arguments[i].Value = command.Arguments[i].Value;
		}

		await _logMessageRepository.Update(item).ConfigureAwait(false);

		await _publisher.PublishAsync(new LogMessageUpdated(
			command.Id,
			command.DateTime,
			command.LogLevel,
			command.Category,
			command.State,
			command.Source,
			command.RawMessage,
			command.Message,
			command.Exception,
			command.ExceptionIsValid,
			command.Arguments)).ConfigureAwait(false);
	}

	public async Task DeleteLogMessage(Guid id, Guid userId)
	{
		await _logMessageRepository.Delete(id).ConfigureAwait(false);

		await _publisher.PublishAsync(new LogMessageDeleted(id)).ConfigureAwait(false);
	}

	public async Task<LogMessage?> GetItemAsync(Guid id, Guid userId)
	{
		var item = await _logMessageRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			return null;
		}

		return new LogMessage
		{
			Id = item.Id,
			DateTime = item.DateTime,
			LogLevel = item.LogLevel,
			Category = item.Category,
			State = item.State,
			Source = item.Source,
			RawMessage = item.RawMessage,
			Message = item.Message,
			Exception = item.Exception,
			ExceptionIsValid = item.ExceptionIsValid,
			Arguments = item.Arguments,
		};
	}

	public async IAsyncEnumerable<LogMessageListItem> GetItemsForUserAsync(LogMessageFilter filter, Guid userId)
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

		await foreach (var item in enumerable.ConfigureAwait(false))
		{
			yield return new LogMessageListItem
			{
				Id = item.Id,
				DateTime = item.DateTime,
				LogLevel = item.LogLevel,
			};
		}
	}

	public async IAsyncEnumerable<LogMessageListItem> GetItemsForUserAsync(Guid userId)
	{
		await foreach (var item in _logMessageRepository.GetAll().ConfigureAwait(false))
		{
			yield return new LogMessageListItem
			{
				Id = item.Id,
				DateTime = item.DateTime,
				LogLevel = item.LogLevel,
			};
		}
	}
}