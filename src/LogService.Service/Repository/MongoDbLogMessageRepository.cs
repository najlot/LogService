using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LogService.Service.Configuration;
using LogService.Service.Model;

namespace LogService.Service.Repository
{
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

		public async Task<LogMessageModel> Get(Guid id)
		{
			var result = await _collection.FindAsync(item => item.Id == id).ConfigureAwait(false);
			return await result.FirstOrDefaultAsync().ConfigureAwait(false);
		}

		public async Task Insert(LogMessageModel[] models)
		{
			foreach (var model in models)
			{
				await _collection.InsertOneAsync(model).ConfigureAwait(false);
			}
		}

		public async Task Insert(LogMessageModel model)
		{
			await _collection.InsertOneAsync(model).ConfigureAwait(false);
		}

		public async Task Update(LogMessageModel model)
		{
			await _collection.FindOneAndReplaceAsync(item => item.Id == model.Id, model).ConfigureAwait(false);
		}

		public async Task Delete(Guid id)
		{
			await _collection.DeleteOneAsync(item => item.Id == id).ConfigureAwait(false);
		}
	}
}