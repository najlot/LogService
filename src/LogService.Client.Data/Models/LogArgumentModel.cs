using System;
using System.Collections.Generic;
using LogService.Contracts;

namespace LogService.Client.Data.Models
{
	public class LogArgumentModel : ILogArgument
	{
		public int Id { get; set; }

		public string Key { get; set; } = string.Empty;
		public string Value { get; set; } = string.Empty;
	}
}
