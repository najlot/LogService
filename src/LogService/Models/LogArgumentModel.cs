using System;
using System.Collections.Generic;
using LogService;

namespace LogService.Models;

public class LogArgumentModel
{
	public int Id { get; set; }

	public string Key { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}