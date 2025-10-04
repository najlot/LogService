﻿using System;

namespace LogService.Events;

public class UserCreated(
	Guid id,
	string username,
	string eMail)
{
	public Guid Id { get; } = id;
	public string Username { get; } = username;
	public string EMail { get; } = eMail;
}