using System;
using System.Collections.Generic;

namespace LogService.Contracts.Events
{
	public class LogMessageDeleted
	{
		public Guid Id { get; set; }

		private LogMessageDeleted() { }

		public LogMessageDeleted(Guid id)
		{
			Id = id;
		}
	}
}
