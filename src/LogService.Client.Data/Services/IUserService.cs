using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;

namespace LogService.Client.Data.Services
{
	public interface IUserService : IDisposable
	{
		UserModel CreateUser();
		Task<bool> AddItemAsync(UserModel item);
		Task<IEnumerable<UserListItemModel>> GetItemsAsync(bool forceRefresh = false);
		Task<UserModel> GetItemAsync(Guid id);
		Task<bool> UpdateItemAsync(UserModel item);
		Task<bool> DeleteItemAsync(Guid id);
		Task<UserModel> GetCurrentUserAsync();
	}
}
