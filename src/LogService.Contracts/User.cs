﻿using System;

namespace LogService.Contracts;

public class User
{
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public UserSettings Settings { get; set; } = new();
}