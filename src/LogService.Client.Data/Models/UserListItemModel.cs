using System;

namespace LogService.Client.Data.Models;

public class UserListItemModel
{
	public Guid Id { get; set; }

	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
}