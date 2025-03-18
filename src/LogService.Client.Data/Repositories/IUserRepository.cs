using System;
using System.Threading.Tasks;
using LogService.Client.Data.Models;

namespace LogService.Client.Data.Repositories;

public interface IUserRepository : IDisposable
{
	Task<UserModel> GetCurrentUserAsync();

	Task<UserListItemModel[]> GetItemsAsync();

	Task UpdateSettingsAsync(UserSettingsModel item);

	Task<UserModel> GetItemAsync(Guid id);

	Task AddItemAsync(UserModel item);

	Task UpdateItemAsync(UserModel item);

	Task DeleteItemAsync(Guid id);
}