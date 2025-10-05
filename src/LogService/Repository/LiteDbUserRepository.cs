using LiteDB;
using LogService.Model;

namespace LogService.Repository;

public sealed class LiteDbUserRepository(LiteDbContext context) : IUserRepository
{
	private readonly ILiteCollection<UserModel> _collection = context.Users;

	public IAsyncEnumerable<UserModel> GetAll()
	{
		return _collection.FindAll().ToAsyncEnumerable();
	}

	public Task<UserModel?> Get(Guid id)
	{
		return Task.FromResult<UserModel?>(_collection.FindById(id));
	}

	public Task<UserModel?> Get(string username)
	{
		return Task.FromResult<UserModel?>(_collection.FindOne(x => x.Username == username && x.IsActive));
	}

	public Task Insert(UserModel model)
	{
		_collection.Insert(model);
		context.Checkpoint();
		return Task.CompletedTask;
	}

	public Task Update(UserModel model)
	{
		_collection.Update(model);
		context.Checkpoint();
		return Task.CompletedTask;
	}
}