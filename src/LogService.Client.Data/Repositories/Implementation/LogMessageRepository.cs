using Cosei.Client.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogService.Contracts;
using LogService.Client.Data.Models;
using LogService.Client.Data.Mappings;
using LogService.Contracts.Commands;
using LogService.Client.Data.Identity;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Repositories.Implementation
{
	public class LogMessageRepository : ILogMessageRepository
	{
		private readonly IRequestClient _client;
		private readonly ITokenProvider _tokenProvider;
		private IEnumerable<LogMessageListItemModel> items;

		public LogMessageRepository(IRequestClient client, ITokenProvider tokenProvider)
		{
			_tokenProvider = tokenProvider;
			_client = client;
			items = new List<LogMessageListItemModel>();
		}

		public async Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(bool forceRefresh = false)
		{
			if (forceRefresh)
			{
				var token = await _tokenProvider.GetToken();

				var headers = new Dictionary<string, string>
				{
					{ "Authorization", $"Bearer {token}" }
				};

				items = await _client.GetAsync<List<LogMessageListItemModel>>("api/LogMessage", headers);
			}

			return items;
		}

		public async Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(LogMessageFilter filter)
		{
			var token = await _tokenProvider.GetToken();

			var headers = new Dictionary<string, string>
			{
				{ "Authorization", $"Bearer {token}" }
			};

			return await _client.PostAsync<List<LogMessageListItemModel>, LogMessageFilter>("api/LogMessage/ListFiltered", filter, headers);
		}

		public async Task<LogMessageModel> GetItemAsync(Guid id)
		{
			if (id != Guid.Empty)
			{
				var token = await _tokenProvider.GetToken();

				var headers = new Dictionary<string, string>
				{
					{ "Authorization", $"Bearer {token}" }
				};

				return await _client.GetAsync<LogMessageModel>($"api/LogMessage/{id}", headers);
			}

			return null;
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				return false;
			}

			var token = await _tokenProvider.GetToken();

			var headers = new Dictionary<string, string>
			{
				{ "Authorization", $"Bearer {token}" }
			};

			var response = await _client.DeleteAsync($"api/LogMessage/{id}", headers);
			response.EnsureSuccessStatusCode();

			return true;
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				disposedValue = true;

				if (disposing)
				{
					_client.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}