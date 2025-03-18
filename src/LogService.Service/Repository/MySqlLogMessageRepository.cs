using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogService.Service.Model;

namespace LogService.Service.Repository;

public class MySqlLogMessageRepository : ILogMessageRepository
{
	private readonly MySqlDbContext _context;

	public MySqlLogMessageRepository(MySqlDbContext context)
	{
		_context = context;
	}

	public IAsyncEnumerable<LogMessageModel> GetAll(Guid userId)
	{
		return _context.LogMessages
			.Where(m => m.CreatedBy == userId)
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public IQueryable<LogMessageModel> GetAllQueryable()
	{
		return _context
			.LogMessages
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<LogMessageModel?> Get(Guid id)
	{
		var e = await _context.LogMessages.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (e == null)
		{
			return null;
		}


		return e;
	}

	public async Task Insert(LogMessageModel[] models)
	{
		foreach (var model in models)
		{
			foreach (var entry in model.Arguments)
			{
				entry.Id = 0;
			}
		}

		await _context.AddRangeAsync(models).ConfigureAwait(false);
		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Insert(LogMessageModel model)
	{

		foreach (var entry in model.Arguments)
		{
			entry.Id = 0;
		}

		await _context.LogMessages.AddAsync(model).ConfigureAwait(false);

		await _context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Delete(Guid id)
	{
		var model = await _context.LogMessages.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);

		if (model != null)
		{
			_context.LogMessages.Remove(model);
			await _context.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}