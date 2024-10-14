using System;
using System.Collections.Generic;

namespace LogService.Contracts.Events
{
	public class UserCreated
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string EMail { get; set; }
		public string Password { get; set; }

		private UserCreated() { }

		public UserCreated(
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
