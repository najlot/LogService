﻿namespace LogService.Contracts;

public class LogArgument
{
	public int Id { get; set; }
	public string Key { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}