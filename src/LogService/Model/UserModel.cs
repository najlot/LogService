using MongoDB.Bson.Serialization.Attributes;

namespace LogService.Model;

[BsonIgnoreExtraElements]
public class UserModel
{
	[BsonId]
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
	public byte[] PasswordHash { get; set; } = [];
	public bool IsActive { get; set; }
	public UserSettingsModel Settings { get; set; } = new();
}