using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.Data.Repositories;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Services.Implementation;

public sealed class LogMessageService : ILogMessageService
{
	private readonly ILogMessageRepository _repository;

	public LogMessageService(ILogMessageRepository repository)
	{
		_repository = repository;
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
			Arguments = []
		};
	}

	public async Task AddItemAsync(LogMessageModel item)
	{
		await _repository.AddItemAsync(item);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		await _repository.DeleteItemAsync(id);
	}

	public async Task<LogMessageModel> GetItemAsync(Guid id)
	{
		return await _repository.GetItemAsync(id);
	}

	public async Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync()
	{
		return await _repository.GetItemsAsync();
	}

	public async Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(LogMessageFilter filter)
	{
		return await _repository.GetItemsAsync(filter);
	}

	public async Task UpdateItemAsync(LogMessageModel item)
	{
		await _repository.UpdateItemAsync(item);
	}

	public void Dispose() => _repository.Dispose();
}