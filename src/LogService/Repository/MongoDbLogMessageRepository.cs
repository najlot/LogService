using MongoDB.Driver;
using LogService.Model;

namespace LogService.Repository;

public class MongoDbLogMessageRepository : ILogMessageRepository
{
	private readonly IMongoCollection<LogMessageModel> _collection;
	private readonly MongoDbContext _context;

	public MongoDbLogMessageRepository(MongoDbContext context)
	{
		_context = context;
		_collection = _context.Database.GetCollection<LogMessageModel>(nameof(LogMessageModel)[0..^5]);
	}

	public async IAsyncEnumerable<LogMessageModel> GetAll(Guid userId)
	{
		var items = await _collection.FindAsync(m => m.CreatedBy == userId).ConfigureAwait(false);

		while (await items.MoveNextAsync().ConfigureAwait(false))
		{
			foreach (var item in items.Current)
			{
				yield return item;
			}
		}
	}

	public IQueryable<LogMessageModel> GetAllQueryable() => _collection.AsQueryable();

	public async Task<LogMessageModel?> Get(Guid id)
	{
		var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
		return await result.FirstOrDefaultAsync().ConfigureAwait(false);
	}

	public async Task Insert(LogMessageModel[] models)
	{
		await _collection.InsertManyAsync(models).ConfigureAwait(false);
	}

	public async Task Delete(Guid[] ids)
	{
		await _collection.DeleteManyAsync(item => ids.Contains(item.Id)).ConfigureAwait(false);
	}
}