using System.Security.Cryptography;
using System.Text;
using LogService.Identity;
using LogService.Model;
using LogService.Repository;

namespace LogService.Services;

public class UserService(IUserRepository userRepository, IAuthenticationService authenticationStateProvider) : IUserService
{
	public async Task CreateUser(Guid id, string username, string email, string password)
	{
		username = username.Normalize().ToLower();
		password = password.Trim();

		var user = await userRepository.Get(username).ConfigureAwait(false);
		if (user != null)
		{
			throw new InvalidOperationException("User already exists!");
		}

		if (password.Length < 6)
		{
			throw new InvalidOperationException("Password too short!");
		}

		var passwordBytes = Encoding.UTF8.GetBytes(password);
		var passwordHash = SHA256.HashData(passwordBytes);

		var item = new UserModel
		{
			Id = id,
			EMail = email,
			Username = username,
			PasswordHash = passwordHash,
			IsActive = true,
			Settings = new UserSettingsModel
			{
				LogRetentionDays = 7
			}
		};

		await userRepository.Insert(item).ConfigureAwait(false);
	}

	public async Task<UserModel?> GetUserModelFromName(string username)
	{
		username = username.Normalize().ToLower();
		var user = await userRepository.Get(username).ConfigureAwait(false);
		return user;
	}

	public async Task<UserModel> GetCurrentUserAsync()
	{
		var userId = await authenticationStateProvider.GetUserIdAsync();

		var user = await userRepository.Get(userId).ConfigureAwait(false);
		if (user == null)
		{
			throw new InvalidOperationException("User not found");
		}

		return user;
	}

	public async Task UpdateItemAsync(UserModel user, string? newPassword = null)
	{
		newPassword = newPassword?.Trim();

		if (!string.IsNullOrWhiteSpace(newPassword))
		{
			if (newPassword.Length < 6)
			{
				throw new InvalidOperationException("Password too short!");
			}

			var passwordBytes = Encoding.UTF8.GetBytes(newPassword);
			user.PasswordHash = SHA256.HashData(passwordBytes);
		}

		await userRepository.Update(user).ConfigureAwait(false);
	}

	public async Task UpdateSettingsAsync(UserSettingsModel settings)
	{
		var user = await GetCurrentUserAsync();
		user.Settings.LogRetentionDays = settings.LogRetentionDays;
		await userRepository.Update(user).ConfigureAwait(false);
	}
}