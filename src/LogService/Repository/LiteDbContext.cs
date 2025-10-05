using LiteDB;
using LogService.Model;
using LogService.Configuration;

namespace LogService.Repository;

public sealed class LiteDbContext : IDisposable
{
	private static bool _indexesInitialized = false;

	private readonly LiteDatabase _db;

	public ILiteCollection<LogMessageModel> LogMessages { get; private set; }
	public ILiteCollection<UserModel> Users { get; private set; }

	public LiteDbContext(LiteDbConfiguration configuration)
	{
		// Ensure the directory exists
		var directory = Path.GetDirectoryName(configuration.DatabasePath);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}

		_db = new LiteDatabase(configuration.DatabasePath);
		LogMessages = _db.GetCollection<LogMessageModel>(nameof(LogMessages));
		Users = _db.GetCollection<UserModel>(nameof(Users));

		if (!_indexesInitialized)
		{
			_indexesInitialized = true;
			InitializeUsersIndexes();
			InitializeLogMessagesIndexes();
		}
	}

	private void InitializeUsersIndexes()
	{
		Users.EnsureIndex(x => x.Username);
		Users.EnsureIndex(x => x.IsActive);
	}

	private void InitializeLogMessagesIndexes()
	{
		LogMessages.EnsureIndex(x => x.CreatedBy);
		LogMessages.EnsureIndex(x => x.DateTime);
		LogMessages.EnsureIndex(x => x.LogLevel);
		LogMessages.EnsureIndex(x => x.Source);
		LogMessages.EnsureIndex(x => x.Category);
	}

	public void Checkpoint() => _db.Checkpoint();

	public void Dispose() => _db.Dispose();
}
