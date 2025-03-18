using Cosei.Client.Base;
using Najlot.Map;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.Data.Identity;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Filters;
using LogService.Contracts.ListItems;

namespace LogService.Client.Data.Repositories.Implementation;

public class LogMessageRepository : ILogMessageRepository
{
	private readonly IRequestClient _client;
	private readonly ITokenProvider _tokenProvider;
	private readonly IMap _map;

	public LogMessageRepository(IRequestClient client, ITokenProvider tokenProvider, IMap map)
	{
		_tokenProvider = tokenProvider;
		_client = client;
		_map = map;
	}

	public async Task<LogMessageListItemModel[]> GetItemsAsync()
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var items = await _client.GetAsync<LogMessageListItem[]>("api/LogMessage", headers);
		return _map.From<LogMessageListItem>(items).ToArray<LogMessageListItemModel>();
	}

	public async Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var items = await _client.PostAsync<List<LogMessageListItem>, LogMessageFilter>("api/LogMessage/ListFiltered", filter, headers);
		return _map.From<LogMessageListItem>(items).ToArray<LogMessageListItemModel>();
	}

	public async Task<LogMessageModel> GetItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var item = await _client.GetAsync<LogMessage>($"api/LogMessage/{id}", headers);
		return _map.From(item).To<LogMessageModel>();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var response = await _client.DeleteAsync($"api/LogMessage/{id}", headers);
		response.EnsureSuccessStatusCode();
	}

	public void Dispose() => _client.Dispose();
}