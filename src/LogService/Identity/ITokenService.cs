using System.Threading.Tasks;

namespace LogService.Identity;

public interface ITokenService
{
	Task<string> CreateToken(string username, string password);
}