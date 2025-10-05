using Najlot.Map;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.Filters;
using LogService.Model;
using LogService.Identity;
using LogService.Repository;

namespace LogService.Services;

public class LogMessageService : ILogMessageService
{
	private readonly ILogMessageRepository _logMessageRepository;
	private readonly IMessenger _messenger;
	private readonly IMap _map;
	private readonly IAuthenticationService _authenticationService;

	public LogMessageService(
		ILogMessageRepository logMessageRepository,
		IMessenger messenger,
		IMap map,
		IAuthenticationService authenticationService)
	{
		_logMessageRepository = logMessageRepository;
		_messenger = messenger;
		_map = map;
		_authenticationService = authenticationService;
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
		await _messenger.SendAsync(messages).ConfigureAwait(false);
	}

	public async Task<LogMessageModel?> GetItemAsync(Guid id)
	{
		var userId = await _authenticationService.GetUserIdAsync();
		var item = await _logMessageRepository.Get(id).ConfigureAwait(false);

		if (item == null || item.CreatedBy != userId)
		{
			return null;
		}

		return item;
	}

	public async Task<LogMessageListItemModel[]> GetItemsAsync()
	{
		var userId = await _authenticationService.GetUserIdAsync();
		var queryable = _logMessageRepository
			.GetAllQueryable()
			.Where(e => e.CreatedBy == userId)
			.OrderByDescending(e => e.DateTime);

		return _map.From<LogMessageModel>(queryable).ToArray<LogMessageListItemModel>();
	}

	public async Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter)
	{
		var userId = await _authenticationService.GetUserIdAsync();
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

		return _map.From<LogMessageModel>(sourceItems).ToArray<LogMessageListItemModel>();
	}
}