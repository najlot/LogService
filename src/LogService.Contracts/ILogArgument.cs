using System;
using System.Collections.Generic;

namespace LogService.Contracts
{
	public interface ILogArgument
	{
		int Id { get; set; }
		string Key { get; set; }
		string Value { get; set; }
	}
}
