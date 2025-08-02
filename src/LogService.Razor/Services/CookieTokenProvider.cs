using LogService.Client.Data.Identity;
using System.Security.Claims;
using System.Text;
using System.Web;
using Cosei.Client.Base;

namespace LogService.Razor.Services;

public class CookieTokenProvider : ITokenProvider
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly ILogger<CookieTokenProvider> _logger;
	private readonly IRequestClient _requestClient;

	public CookieTokenProvider(IHttpContextAccessor httpContextAccessor, ILogger<CookieTokenProvider> logger, IRequestClient requestClient)
	{
		_httpContextAccessor = httpContextAccessor;
		_logger = logger;
		_requestClient = requestClient;
	}

	public async Task<string> GetToken()
	{
		await Task.CompletedTask;
		var context = _httpContextAccessor.HttpContext;
		if (context?.User?.Identity?.IsAuthenticated == true)
		{
			var token = context.User.FindFirst("access_token")?.Value;
			if (!string.IsNullOrEmpty(token))
			{
				return token;
			}
		}
		throw new System.Security.Authentication.AuthenticationException();
	}

	public async Task<string> GetServiceToken(string source, DateTime validUntil)
	{
		var token = await GetToken();
		if (string.IsNullOrEmpty(token))
		{
			throw new System.Security.Authentication.AuthenticationException();
		}

		var headers = new Dictionary<string, string>
		{
			{ "Authorization", $"Bearer {token}" }
		};

		var encodedSource = HttpUtility.UrlEncode(source);
		var encodedValidUntil = HttpUtility.UrlEncode(validUntil.ToString("o"));
		var query = $"?source={encodedSource}&validUntil={encodedValidUntil}";
		var response = await _requestClient.GetAsync("api/Auth/ServiceToken" + query, headers);

		if (response.StatusCode >= 200 && response.StatusCode < 300)
		{
			return Encoding.UTF8.GetString(response.Body.ToArray());
		}

		throw new System.Security.Authentication.AuthenticationException();
	}
}