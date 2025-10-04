using System;
using System.Threading.Tasks;

namespace LogService.Identity;

public interface IRegistrationService
{
	Task<RegistrationResult> Register(Guid id, string username, string email, string password);
}