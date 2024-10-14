using System;
using System.Collections.Generic;

namespace LogService.Contracts.Events
{
	public class UserDeleted
	{
		public Guid Id { get; set; }

		private UserDeleted() { }

		public UserDeleted(Guid id)
		{
			Id = id;
		}
	}
}
