using LogService.Configuration;
using LogService.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LogService.Identity;

public class AuthenticationService(
	IUserDataStore userDataStore,
	ServiceConfiguration serviceConfiguration,
	ILogger<AuthenticationService> logger)
	: AuthenticationStateProvider, IAuthenticationService
{
	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			var username = await userDataStore.GetUsername();
			var token = await userDataStore.GetAccessToken();

			if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(username))
			{
				var parameters = TokenService.GetValidationParameters(serviceConfiguration.Secret);
				var tokenHandler = new JwtSecurityTokenHandler();

				// Validate the token
				var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);

				if (validatedToken is JwtSecurityToken jwtToken)
				{
					var validTo = jwtToken.Payload.ValidTo;
					if (validTo > DateTime.UtcNow)
					{
						return await GenerateAuthenticationState(username, jwtToken);
					}
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Could not retrieve authentication state.");
			await LogoutAsync();
		}

		return await GenerateEmptyAuthenticationState();
	}

	public async Task LoginAsync(string username, string token)
	{
		await userDataStore.SetUserData(username, token);
		var securityToken = new JwtSecurityToken(token);
		NotifyAuthenticationStateChanged(GenerateAuthenticationState(username, securityToken));
	}

	public async Task LogoutAsync()
	{
		await userDataStore.SetUserData("", "");

		NotifyAuthenticationStateChanged(GenerateEmptyAuthenticationState());
	}

	private Task<AuthenticationState> GenerateAuthenticationState(string username, JwtSecurityToken securityToken)
	{
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, username)
		};

		// Extract the NameIdentifier claim from the JWT token
		var userIdClaim = securityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		if (userIdClaim != null)
		{
			claims.Add(new Claim(ClaimTypes.NameIdentifier, userIdClaim.Value));
		}

		var claimsIdentity = new ClaimsIdentity(claims, "auth");
		var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
		return Task.FromResult(new AuthenticationState(claimsPrincipal));
	}

	private Task<AuthenticationState> GenerateEmptyAuthenticationState()
	{
		return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
	}
}