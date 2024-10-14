using Cosei.Service.Base;
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

namespace LogService.Service.Services
{
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

		public async Task CreateLogMessages(CreateLogMessage[] commands, string source, Guid userId)
		{
			var items = commands.Select(command =>
			{
				int i = 0;

				return new LogMessageModel()
				{
					Id = Guid.NewGuid(),
					CreatedBy = userId,
					DateTime = command.DateTime,
					LogLevel = (LogLevel)command.LogLevel,
					Category = command.Category ?? "",
					State = command.State ?? "",
					Source = source,
					RawMessage = command.RawMessage ?? "",
					Message = command.Message ?? "",
					Exception = command.Exception ?? "",
					ExceptionIsValid = command.ExceptionIsValid,
					Arguments = command.Arguments.Select(a => new LogArgument()
					{
						Id = i++,
						Key = a.Key,
						Value = a.Value,
					}).ToList(),
				};
			}).ToArray();

			await _logMessageRepository.Insert(items).ConfigureAwait(false);

			foreach (var item in items)
			{
				await _publisher.PublishToUserAsync(userId.ToString(), new LogMessageCreated(
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
					item.Arguments)).ConfigureAwait(false);
			}
		}

		public async Task DeleteLogMessage(Guid id, Guid userId)
		{
			await _logMessageRepository.Delete(id).ConfigureAwait(false);
		}

		public async Task<LogMessage> GetItemAsync(Guid id, Guid userId)
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
			var enumerable = _logMessageRepository.GetAll(userId);

			if (filter.DateTimeFrom != null)
				enumerable = enumerable.Where(e => e.DateTime >= filter.DateTimeFrom);
			if (filter.DateTimeTo != null)
				enumerable = enumerable.Where(e => e.DateTime <= filter.DateTimeTo);
			if (filter.LogLevel != null)
				enumerable = enumerable.Where(e => e.LogLevel == filter.LogLevel);
			if (filter.Category != null)
				enumerable = enumerable.Where(e => e.Category.Contains(filter.Category, StringComparison.OrdinalIgnoreCase));
			if (filter.State != null)
				enumerable = enumerable.Where(e => e.State.Contains(filter.State, StringComparison.OrdinalIgnoreCase));
			if (filter.Source != null)
				enumerable = enumerable.Where(e => e.Source.Contains(filter.Source, StringComparison.OrdinalIgnoreCase));
			if (filter.RawMessage != null)
				enumerable = enumerable.Where(e => e.RawMessage.Contains(filter.RawMessage, StringComparison.OrdinalIgnoreCase));
			if (filter.Message != null)
				enumerable = enumerable.Where(e => e.Message.Contains(filter.Message, StringComparison.OrdinalIgnoreCase));
			if (filter.Exception != null)
				enumerable = enumerable.Where(e => e.Exception.Contains(filter.Exception, StringComparison.OrdinalIgnoreCase));
			if (filter.ExceptionIsValid != null)
				enumerable = enumerable.Where(e => e.ExceptionIsValid == filter.ExceptionIsValid);

			enumerable = enumerable.OrderByDescending(e => e.DateTime);

			await foreach (var item in enumerable.ConfigureAwait(false))
			{
				yield return new LogMessageListItem
				{
					Id = item.Id,
					DateTime = item.DateTime,
					LogLevel = item.LogLevel,
					Message = item.Message,
				};
			}
		}

		public async IAsyncEnumerable<LogMessageListItem> GetItemsForUserAsync(Guid userId)
		{
			var enumerable = _logMessageRepository.GetAll(userId);
			enumerable = enumerable.OrderByDescending(e => e.DateTime);

			await foreach (var item in enumerable.ConfigureAwait(false))
			{
				yield return new LogMessageListItem
				{
					Id = item.Id,
					DateTime = item.DateTime,
					LogLevel = item.LogLevel,
					Message = item.Message,
				};
			}
		}
	}
}