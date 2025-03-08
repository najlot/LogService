using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.ListItems;
using LogService.Service.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogService.Service.Services;

public interface IUserService
{
	Task<User?> GetItemAsync(Guid id);
	IAsyncEnumerable<UserListItem> GetItemsForUserAsync(Guid userId);
	Task<UserModel?> GetUserModelFromName(string username);

	Task CreateUser(CreateUser command, Guid userId);
	Task UpdateUser(UpdateUser command, Guid userId);
	Task DeleteUser(Guid id, Guid userId);
}