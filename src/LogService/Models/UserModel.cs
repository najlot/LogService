using System;
using System.Collections.Generic;
using LogService;

namespace LogService.Models;

public class UserModel
{
	public Guid Id { get; set; }

	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;

	public UserSettingsModel Settings { get; set; } = new UserSettingsModel();
}