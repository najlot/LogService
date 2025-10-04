using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using LogService.Configuration;
using LogService.Model;

namespace LogService.Repository;

public class LiteDbLogMessageRepository : ILogMessageRepository
{
	private readonly string _connectionString;

	public LiteDbLogMessageRepository(LiteDbConfiguration configuration)
	{
		// Ensure the directory exists
		var directory = Path.GetDirectoryName(configuration.DatabasePath);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}
		
		_connectionString = $"Filename={configuration.DatabasePath};Connection=shared";
	}

	public async IAsyncEnumerable<LogMessageModel> GetAll(Guid userId)
	{
		await Task.CompletedTask;
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<LogMessageModel>("logMessages");
		
		foreach (var item in collection.Find(x => x.CreatedBy == userId))
		{
			yield return item;
		}
	}

	public IQueryable<LogMessageModel> GetAllQueryable()
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<LogMessageModel>("logMessages");
		return collection.FindAll().AsQueryable();
	}

	public Task<LogMessageModel?> Get(Guid id)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<LogMessageModel>("logMessages");
		var item = collection.FindById(id);
		return Task.FromResult<LogMessageModel?>(item);
	}

	public Task Insert(LogMessageModel[] models)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<LogMessageModel>("logMessages");
		collection.InsertBulk(models);
		return Task.CompletedTask;
	}

	public Task Insert(LogMessageModel model)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<LogMessageModel>("logMessages");
		collection.Insert(model.Id, model);
		return Task.CompletedTask;
	}

	public Task Delete(Guid id)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<LogMessageModel>("logMessages");
		collection.Delete(id);
		return Task.CompletedTask;
	}
}
