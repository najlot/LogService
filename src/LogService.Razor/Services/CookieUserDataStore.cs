using LogService.Client.Data.Identity;
using System.Security.Claims;

namespace LogService.Razor.Services;

public class CookieUserDataStore : IUserDataStore
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly ILogger<CookieUserDataStore> _logger;

	public CookieUserDataStore(IHttpContextAccessor httpContextAccessor, ILogger<CookieUserDataStore> logger)
	{
		_httpContextAccessor = httpContextAccessor;
		_logger = logger;
	}

	public async Task<string?> GetAccessToken()
	{
		await Task.CompletedTask;
		var context = _httpContextAccessor.HttpContext;
		if (context?.User?.Identity?.IsAuthenticated == true)
		{
			return context.User.FindFirst("access_token")?.Value;
		}
		return null;
	}

	public async Task SetAccessToken(string token)
	{
		await Task.CompletedTask;
		// Token is set during login, this is handled by the authentication middleware
		_logger.LogDebug("Token set operation called - handled by authentication middleware");
	}

	public async Task<string?> GetUsername()
	{
		await Task.CompletedTask;
		var context = _httpContextAccessor.HttpContext;
		if (context?.User?.Identity?.IsAuthenticated == true)
		{
			return context.User.FindFirst(ClaimTypes.Name)?.Value;
		}
		return null;
	}

	public async Task SetUsername(string username)
	{
		await Task.CompletedTask;
		// Username is set during login, this is handled by the authentication middleware
		_logger.LogDebug("Username set operation called - handled by authentication middleware");
	}

	public async Task SetUserData(string username, string token)
	{
		await Task.CompletedTask;
		// User data is set during login, this is handled by the authentication middleware
		_logger.LogDebug("UserData set operation called - handled by authentication middleware");
	}
}