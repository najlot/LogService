namespace LogService.Identity;

public interface IUserDataStore
{
	Task<string?> GetAccessToken();
	Task<string?> GetUsername();
	Task SetAccessToken(string token);
	Task SetUserData(string username, string token);
	Task SetUsername(string username);
}