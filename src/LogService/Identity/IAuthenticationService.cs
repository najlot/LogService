using Microsoft.AspNetCore.Components.Authorization;

namespace LogService.Identity;

public interface IAuthenticationService
{
	Task<AuthenticationState> GetAuthenticationStateAsync();

	Task LoginAsync(string username, string token);

	Task LogoutAsync();
}