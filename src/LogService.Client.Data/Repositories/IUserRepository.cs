using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;

namespace LogService.Client.Data.Repositories
{
	public interface IUserRepository : IDisposable
	{
		Task<bool> AddItemAsync(UserModel item);

		Task<bool> UpdateItemAsync(UserModel item);

		Task<bool> DeleteItemAsync(Guid id);

		Task<UserModel> GetItemAsync(Guid id);

		Task<IEnumerable<UserListItemModel>> GetItemsAsync(bool forceRefresh = false);

		Task<UserModel> GetCurrentUserAsync();
	}
}
