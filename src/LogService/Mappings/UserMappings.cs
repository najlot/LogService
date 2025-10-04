using Najlot.Map;
using Najlot.Map.Attributes;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using LogService.Model;

namespace LogService.Mappings;

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
	[MapIgnoreProperty(nameof(to.Settings))]
	public void MapToModel(CreateUser from, UserModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}

	[MapIgnoreProperty(nameof(to.Password))]
	public void MapFromModel(IMap map, UserModel from, User to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
		to.Settings = map.From(from.Settings).To<UserSettings>();
	}

	public void MapFromModel(UserSettingsModel from, UserSettings to)
	{
		to.LogRetentionDays = from.LogRetentionDays;
	}

	public void MapFromModel(UserModel from, UserListItem to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}
}