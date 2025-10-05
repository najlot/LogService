using Microsoft.IdentityModel.Tokens;

namespace LogService.Services;

public interface ITokenService
{
	string GetServiceToken(string username, Guid userId, string source, DateTime validUntil);
	Task<string?> GetToken(string username, string password);
	TokenValidationParameters GetValidationParameters();
}