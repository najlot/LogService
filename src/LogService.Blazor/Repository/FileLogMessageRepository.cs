﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LogService.Blazor.Configuration;
using LogService.Blazor.Model;

namespace LogService.Blazor.Repository;

public class FileLogMessageRepository : ILogMessageRepository
{
	private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
	private readonly string _storagePath;

	public FileLogMessageRepository(FileConfiguration configuration)
	{
		_storagePath = configuration.LogMessagesPath;
		Directory.CreateDirectory(_storagePath);
	}

	public async IAsyncEnumerable<LogMessageModel> GetAll(Guid userId)
	{
		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<LogMessageModel>(text, _options);
			if (item is not null && item.CreatedBy == userId)
			{
				yield return item;
			}
		}
	}

	public IQueryable<LogMessageModel> GetAllQueryable()
	{
		var items = new List<LogMessageModel>();

		foreach (var path in Directory.GetFiles(_storagePath))
		{
			var bytes = File.ReadAllBytes(path);
			var text = Encoding.UTF8.GetString(bytes);
			var item = JsonSerializer.Deserialize<LogMessageModel>(text, _options);
			if (item is not null)
			{
				items.Add(item);
			}
		}

		return items.AsQueryable();
	}

	public async Task<LogMessageModel?> Get(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());

		if (!File.Exists(path))
		{
			return null;
		}

		var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
		var item = JsonSerializer.Deserialize<LogMessageModel>(bytes, _options);

		return item;
	}

	public async Task Insert(LogMessageModel[] models)
	{
		foreach (var model in models)
		{
			await Insert(model).ConfigureAwait(false);
		}
	}

	public async Task Insert(LogMessageModel model)
	{
		var path = Path.Combine(_storagePath, model.Id.ToString());
		var bytes = JsonSerializer.SerializeToUtf8Bytes(model);
		await File.WriteAllBytesAsync(path, bytes).ConfigureAwait(false);
	}

	public Task Delete(Guid id)
	{
		var path = Path.Combine(_storagePath, id.ToString());
		File.Delete(path);
		return Task.CompletedTask;
	}
}