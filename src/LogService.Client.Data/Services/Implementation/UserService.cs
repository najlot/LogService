using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.Data.Repositories;

namespace LogService.Client.Data.Services.Implementation
{
	public class UserService : IUserService
	{
		private IUserRepository _store;

		public UserService(IUserRepository dataStore)
		{
			_store = dataStore;
		}

		public UserModel CreateUser()
		{
			return new UserModel()
			{
				Id = Guid.NewGuid(),
				Username = "",
				EMail = "",
				Password = "",
			};
		}

		public async Task<bool> AddItemAsync(UserModel item)
		{
			return await _store.AddItemAsync(item);
		}

		public async Task<bool> DeleteItemAsync(Guid id)
		{
			return await _store.DeleteItemAsync(id);
		}

		public async Task<UserModel> GetCurrentUserAsync()
		{
			return await _store.GetCurrentUserAsync();
		}

		public async Task<UserModel> GetItemAsync(Guid id)
		{
			return await _store.GetItemAsync(id);
		}

		public async Task<IEnumerable<UserListItemModel>> GetItemsAsync(bool forceRefresh = false)
		{
			return await _store.GetItemsAsync(forceRefresh);
		}

		public async Task<bool> UpdateSettingsAsync(UserSettingsModel item)
		{
			return await _store.UpdateSettingsAsync(item);
		}

		public async Task<bool> UpdateItemAsync(UserModel item)
		{
			return await _store.UpdateItemAsync(item);
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
					_store?.Dispose();
					_store = null;
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