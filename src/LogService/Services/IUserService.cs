using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.ListItems;
using LogService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogService.Services;

public interface IUserService
{
	Task<User?> GetItem(Guid id);
	IAsyncEnumerable<UserListItem> GetItemsForUser(Guid userId);
	Task<UserModel?> GetUserModelFromName(string username);

	Task CreateUser(CreateUser command, Guid userId);
	Task UpdateUser(UpdateUser command, Guid userId);
	Task DeleteUser(Guid id, Guid userId);

	Task UpdateUserSettings(UpdateUserSettings command, Guid userId);
}