using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.ClientBase.Models;

namespace LogService.ClientBase.Services;

public interface IProfilesService
{
	Task<List<ProfileBase>> LoadAsync();
	Task RemoveAsync(ProfileBase profile);
	Task SaveAsync(List<ProfileBase> profiles);
}