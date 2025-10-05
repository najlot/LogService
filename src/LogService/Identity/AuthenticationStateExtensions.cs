using System.Security.Claims;

namespace LogService.Identity;

public static class AuthenticationStateExtensions
{
	/// <summary>
	/// Extracts the user ID from the authentication state.
	/// </summary>
	/// <param name="authenticationService">The authentication service</param>
	/// <returns>The user ID as a Guid</returns>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated or the user ID claim is invalid</exception>
	public static async Task<Guid> GetUserIdAsync(this IAuthenticationService authenticationService)
	{
		var authState = await authenticationService.GetAuthenticationStateAsync();
		var userIdClaim = authState.User.FindFirst(ClaimTypes.NameIdentifier);
		
		if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
		{
			throw new UnauthorizedAccessException("User not authenticated");
		}

		return userId;
	}
}