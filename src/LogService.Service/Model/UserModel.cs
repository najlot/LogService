using MongoDB.Bson.Serialization.Attributes;
using LogService.Contracts;
using System;
using System.Collections.Generic;

namespace LogService.Service.Model
{
	[BsonIgnoreExtraElements]
	public class UserModel
	{
		[BsonId]
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string EMail { get; set; }
		public byte[] PasswordHash { get; set; }
		public bool IsActive { get; set; }

        public UserSettingsModel Settings { get; set; } = new();
    }
}
