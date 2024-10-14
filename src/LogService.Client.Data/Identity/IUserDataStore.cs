using System.Threading.Tasks;

namespace LogService.Client.Data.Identity
{
	public interface IUserDataStore
	{
		Task<string?> GetAccessToken();

		Task<string?> GetUsername();

		Task SetAccessToken(string token);

		Task SetUsername(string username);

		Task SetUserData(string username, string token);
	}
}