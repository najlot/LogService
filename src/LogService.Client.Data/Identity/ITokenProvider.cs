using System.Threading.Tasks;

namespace LogService.Client.Data.Identity
{
	public interface ITokenProvider
	{
		Task<string> GetToken();
	}
}
