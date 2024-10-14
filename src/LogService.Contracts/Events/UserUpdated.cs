using System;
using System.Collections.Generic;

namespace LogService.Contracts.Events
{
	public class UserUpdated
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string EMail { get; set; }
		public string Password { get; set; }

		private UserUpdated() { }

		public UserUpdated(
			Guid id,
			string username,
			string eMail)
		{
			Id = id;
			Username = username;
			EMail = eMail;
		}
	}
}
