using LogService.Client.Data.Identity;
using System.Security.Claims;

namespace LogService.Razor.Services;

public class CookieTokenProvider : ITokenProvider
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly ILogger<CookieTokenProvider> _logger;

	public CookieTokenProvider(IHttpContextAccessor httpContextAccessor, ILogger<CookieTokenProvider> logger)
	{
		_httpContextAccessor = httpContextAccessor;
		_logger = logger;
	}

	public async Task<string?> GetToken()
	{
		await Task.CompletedTask;
		var context = _httpContextAccessor.HttpContext;
		if (context?.User?.Identity?.IsAuthenticated == true)
		{
			return context.User.FindFirst("access_token")?.Value;
		}
		return null;
	}

	public async Task<string?> GetServiceToken(string source, DateTime validUntil)
	{
		// For service tokens, we still need to call the API
		// This is different from the access token stored in cookies
		var token = await GetToken();
		if (token == null) return null;

		// TODO: Implement service token generation call to API
		// This would be a call to the LogService API to generate a service token
		// For now, return null to indicate not implemented
		_logger.LogWarning("Service token generation not yet implemented for Razor pages");
		return null;
	}
}