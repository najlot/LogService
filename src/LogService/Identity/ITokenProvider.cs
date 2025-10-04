using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogService.Identity;

public interface ITokenProvider
{
	Task<string> GetToken();
	Task<string> GetServiceToken(string source, DateTime validUntil);
}

public static class TokenProviderExtensions
{
	public static async Task<Dictionary<string, string>> GetAuthorizationHeaders(this ITokenProvider tokenProvider)
	{
		var token = await tokenProvider.GetToken().ConfigureAwait(false);
		return new Dictionary<string, string>
		{
			{ "Authorization", $"Bearer {token}" }
		};
	}
}