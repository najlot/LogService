using Najlot.Map.Attributes;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using LogService.Service.Model;

namespace LogService.Service.Mappings;

internal class UserMappings
{
	public UserCreated MapToCreated(UserModel from) =>
		new(from.Id,
		from.Username,
		from.EMail);

	public UserUpdated MapToUpdated(UserModel from) =>
		new(from.Id,
		from.Username,
		from.EMail);

	[MapIgnoreProperty(nameof(to.PasswordHash))]
	[MapIgnoreProperty(nameof(to.IsActive))]
	public void MapToModel(CreateUser from, UserModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}

	[MapIgnoreProperty(nameof(to.Password))]
	public void MapFromModel(UserModel from, User to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}

	public void MapFromModel(UserModel from, UserListItem to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}
}