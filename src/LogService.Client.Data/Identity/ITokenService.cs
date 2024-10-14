using System.Threading.Tasks;

namespace LogService.Client.Data.Identity
{
	public interface ITokenService
	{
		Task<string> CreateToken(string username, string password);
	}
}