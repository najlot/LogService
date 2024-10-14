using System;
using System.Collections.Generic;

namespace LogService.Contracts
{
	public class LogArgument : ILogArgument
	{
		public int Id { get; set; }
		public string Key { get; set; } = string.Empty;
		public string Value { get; set; } = string.Empty;
	}
}
