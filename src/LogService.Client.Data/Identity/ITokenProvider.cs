using System;
using System.Threading.Tasks;

namespace LogService.Client.Data.Identity
{
	public interface ITokenProvider
	{
		Task<string> GetServiceToken(string source, DateTime validUntil);
		Task<string> GetToken();
	}
}
