using Cosei.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogService.Contracts;
using LogService.Service.Model;
using LogService.Service.Repository;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using System.Text;
using System.Security.Cryptography;

namespace LogService.Service.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IPublisher _publisher;

		public UserService(
			IUserRepository userRepository,
			IPublisher publisher)
		{
			_userRepository = userRepository;
			_publisher = publisher;
		}

		public async Task CreateUser(CreateUser command, Guid userId)
		{
			command.Username = command.Username.Normalize().ToLower();

			var user = await _userRepository.Get(command.Username).ConfigureAwait(false);
			if (user != null)
			{
				throw new InvalidOperationException("User already exists!");
			}

			if (command.Password.Trim().Length < 6)
			{
				throw new InvalidOperationException("Password too short!");
			}

			var passwordBytes = Encoding.UTF8.GetBytes(command.Password);

			var item = new UserModel()
			{
				Id = command.Id,
				Username = command.Username,
				EMail = command.EMail,
				PasswordHash = SHA256.HashData(passwordBytes),
				IsActive = true
			};

			await _userRepository.Insert(item).ConfigureAwait(false);

			await _publisher.PublishAsync(new UserCreated(
				command.Id,
				command.Username,
				command.EMail)).ConfigureAwait(false);
		}

		public async Task UpdateUser(UpdateUser command, Guid userId)
		{
			command.Username = command.Username.Normalize().ToLower();

			var item = await _userRepository.Get(command.Id).ConfigureAwait(false);

			item.Username = item.Username.Normalize().ToLower();

			if (item.Id != userId)
			{
				throw new InvalidOperationException("You must not modify other users!");
			}

			if (item.Username != command.Username)
			{
				throw new InvalidOperationException("Username can not be modified!");
			}

			item.Username = command.Username;
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

			await _publisher.PublishAsync(new UserUpdated(
				command.Id,
				command.Username,
				command.EMail)).ConfigureAwait(false);
		}

		public async Task DeleteUser(Guid id, Guid userId)
		{
			var item = await _userRepository.Get(id).ConfigureAwait(false);

			if (item.Id != userId)
			{
				throw new InvalidOperationException("You must not delete other user!");
			}

			item.IsActive = false;

			await _userRepository.Update(item).ConfigureAwait(false);

			await _publisher.PublishAsync(new UserDeleted(id)).ConfigureAwait(false);
		}

		public async Task<User> GetItemAsync(Guid id)
		{
			var item = await _userRepository.Get(id).ConfigureAwait(false);

			if (item == null)
			{
				return null;
			}

			return new User
			{
				Id = item.Id,
				Username = item.Username,
				EMail = item.EMail,
			};
		}

		public async IAsyncEnumerable<UserListItem> GetItemsForUserAsync(Guid userId)
		{
			await foreach (var item in _userRepository.GetAll().ConfigureAwait(false))
			{
				if (!item.IsActive)
				{
					continue;
				}

				yield return new UserListItem
				{
					Id = item.Id,
					Username = item.Username,
					EMail = item.EMail,
				};
			}
		}

		public async Task<UserModel> GetUserModelFromId(Guid id)
		{
			var user = await _userRepository.Get(id).ConfigureAwait(false);
			return user;
		}

		public async Task<UserModel> GetUserModelFromName(string username)
		{
			username = username.Normalize().ToLower();

			var user = await _userRepository.Get(username).ConfigureAwait(false);
			return user;
		}
	}
}