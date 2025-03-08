using LogService.Client.Data.Services;
using LogService.ClientBase.Models;
using System.Threading.Tasks;

namespace LogService.ClientBase.ProfileHandler;

public interface IProfileHandler
{
	IUserService GetUserService();
	ILogMessageService GetLogMessageService();

	IProfileHandler SetNext(IProfileHandler handler);

	Task SetProfile(ProfileBase profile);
}