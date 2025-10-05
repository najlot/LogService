using LogService.Model;

namespace LogService.Services;

public interface IUserService
{
	Task<UserModel?> GetUserModelFromName(string username);
	Task CreateUser(Guid id, string username, string email, string password);
	Task<UserModel> GetCurrentUserAsync();
	Task UpdateItemAsync(UserModel user, string? newPassword = null);
	Task UpdateSettingsAsync(UserSettingsModel settings);
}