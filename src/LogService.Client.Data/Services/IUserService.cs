using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;

namespace LogService.Client.Data.Services;

public interface IUserService : IDisposable
{
	UserModel CreateUser();
	Task AddItemAsync(UserModel item);
	Task<IEnumerable<UserListItemModel>> GetItemsAsync();
	Task<UserModel> GetItemAsync(Guid id);
	Task UpdateItemAsync(UserModel item);
	Task DeleteItemAsync(Guid id);
	Task<UserModel> GetCurrentUserAsync();
}