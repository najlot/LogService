using LiteDB;
using LogService.Model;

namespace LogService.Repository;

public class LiteDbLogMessageRepository(LiteDbContext context) : ILogMessageRepository
{
	private readonly ILiteCollection<LogMessageModel> _collection = context.LogMessages;

	public IAsyncEnumerable<LogMessageModel> GetAll(Guid userId)
	{
		return _collection.Find(x => x.CreatedBy == userId).ToAsyncEnumerable();
	}

	public IQueryable<LogMessageModel> GetAllQueryable()
	{
		return _collection.Query().ToEnumerable().AsQueryable();
	}

	public Task<LogMessageModel?> Get(Guid id)
	{
		return Task.FromResult<LogMessageModel?>(_collection.FindById(id));
	}

	public Task Insert(LogMessageModel[] models)
	{
		_collection.InsertBulk(models);
		context.Checkpoint();
		return Task.CompletedTask;
	}

	public Task Delete(Guid[] ids)
	{
		foreach (var id in ids)
		{
			_collection.Delete(id);
		}

		return Task.CompletedTask;
	}
}