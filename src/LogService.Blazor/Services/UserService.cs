using Cosei.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Najlot.Map;
using LogService.Contracts;
using LogService.Blazor.Model;
using LogService.Blazor.Repository;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;

namespace LogService.Blazor.Services;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;
	private readonly IPublisher _publisher;
	private readonly IMap _map;

	public UserService(
		IUserRepository userRepository,
		IPublisher publisher,
		IMap map)
	{
		_userRepository = userRepository;
		_publisher = publisher;
		_map = map;
	}

	public async Task CreateUser(CreateUser command, Guid userId)
	{
		var username = command.Username.Normalize().ToLower();

		var user = await _userRepository.Get(username).ConfigureAwait(false);
		if (user != null)
		{
			throw new InvalidOperationException("User already exists!");
		}

		if (command.Password.Trim().Length < 6)
		{
			throw new InvalidOperationException("Password too short!");
		}

		var passwordBytes = Encoding.UTF8.GetBytes(command.Password);
		var passwordHash = SHA256.HashData(passwordBytes);

		var item = _map.From(command).To<UserModel>();
		item.Username = username;
		item.PasswordHash = passwordHash;
		item.IsActive = true;

		await _userRepository.Insert(item).ConfigureAwait(false);

		var message = _map.From(item).To<UserCreated>();
		await _publisher.PublishToUserAsync(userId.ToString(), message).ConfigureAwait(false);
	}

	public async Task UpdateUserSettings(UpdateUserSettings command, Guid userId)
	{
		var item = await _userRepository.Get(userId).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("User not found!");
		}

		item.Settings.LogRetentionDays = command.LogRetentionDays;
		await _userRepository.Update(item);
	}

	public async Task UpdateUser(UpdateUser command, Guid userId)
	{
		var username = command.Username.Normalize().ToLower();

		var item = await _userRepository.Get(command.Id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("User not found!");
		}

		if (item.Id != userId)
		{
			throw new InvalidOperationException("You must not modify other users!");
		}

		if (item.Username != username)
		{
			throw new InvalidOperationException("Username can not be modified!");
		}

		item.EMail = command.EMail;

		if (!string.IsNullOrWhiteSpace(command.Password))
		{
			if (command.Password.Trim().Length < 6)
			{
				throw new InvalidOperationException("Password too short!");
			}

			var passwordBytes = Encoding.UTF8.GetBytes(command.Password);
			item.PasswordHash = SHA256.HashData(passwordBytes);
		}

		await _userRepository.Update(item).ConfigureAwait(false);

		var message = _map.From(item).To<UserUpdated>();
		await _publisher.PublishToUserAsync(userId.ToString(), message).ConfigureAwait(false);
	}

	public async Task DeleteUser(Guid id, Guid userId)
	{
		var item = await _userRepository.Get(id).ConfigureAwait(false);

		if (item == null)
		{
			throw new InvalidOperationException("User not found!");
		}

		if (item.Id != userId)
		{
			throw new InvalidOperationException("You must not delete other user!");
		}

		item.IsActive = false;

		await _userRepository.Update(item).ConfigureAwait(false);

		var message = new UserDeleted(id);
		await _publisher.PublishToUserAsync(userId.ToString(), message).ConfigureAwait(false);
	}

	public async Task<User?> GetItem(Guid id)
	{
		var item = await _userRepository.Get(id).ConfigureAwait(false);
		return _map.FromNullable(item)?.To<User>();
	}

	public IAsyncEnumerable<UserListItem> GetItemsForUser(Guid userId)
	{
		var items = _userRepository.GetAll().Where(u => u.IsActive && u.Id == userId);
		return _map.From(items).To<UserListItem>();
	}

	public async Task<UserModel?> GetUserModelFromName(string username)
	{
		username = username.Normalize().ToLower();
		var user = await _userRepository.Get(username).ConfigureAwait(false);
		return user;
	}
}