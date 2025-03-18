using LogService.Client.Data.Models;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.ListItems;
using Najlot.Map;

namespace LogService.Client.Data.Mappings;

internal sealed class UserMappings
{
	public void MapToModel(UserSettings from, UserSettingsModel to)
	{
		to.LogRetentionDays = from.LogRetentionDays;
	}

	public void MapToModel(UserListItem from, UserListItemModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
	}

	public void MapToModel(IMap map, User from, UserModel to)
	{
		to.Id = from.Id;
		to.Username = from.Username;
		to.EMail = from.EMail;
		to.Password = from.Password;
		to.Settings = map.From(from.Settings).To<UserSettingsModel>();
	}

	public CreateUser MapToCreate(UserModel item) =>
		new(item.Id,
			item.Username,
			item.EMail,
			item.Password);

	public UpdateUser MapToUpdate(UserModel item) =>
		new(item.Id,
			item.Username,
			item.EMail,
			item.Password);
}