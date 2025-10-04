using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using LogService.Configuration;
using LogService.Model;

namespace LogService.Repository;

public class LiteDbUserRepository : IUserRepository
{
	private readonly string _connectionString;

	public LiteDbUserRepository(LiteDbConfiguration configuration)
	{
		// Ensure the directory exists
		var directory = Path.GetDirectoryName(configuration.DatabasePath);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}
		
		_connectionString = $"Filename={configuration.DatabasePath};Connection=shared";
	}

	public async IAsyncEnumerable<UserModel> GetAll()
	{
		await Task.CompletedTask;
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<UserModel>("users");
		
		foreach (var item in collection.FindAll())
		{
			yield return item;
		}
	}

	public Task<UserModel?> Get(Guid id)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<UserModel>("users");
		var item = collection.FindById(id);
		return Task.FromResult<UserModel?>(item);
	}

	public Task<UserModel?> Get(string username)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<UserModel>("users");
		var item = collection.FindOne(x => x.IsActive && x.Username == username);
		return Task.FromResult<UserModel?>(item);
	}

	public Task Insert(UserModel model)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<UserModel>("users");
		collection.Insert(model.Id, model);
		return Task.CompletedTask;
	}

	public Task Update(UserModel model)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<UserModel>("users");
		collection.Update(model.Id, model);
		return Task.CompletedTask;
	}

	public Task Delete(Guid id)
	{
		using var db = new LiteDatabase(_connectionString);
		var collection = db.GetCollection<UserModel>("users");
		collection.Delete(id);
		return Task.CompletedTask;
	}
}
