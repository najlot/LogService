using Microsoft.EntityFrameworkCore;
using LogService.Model;

namespace LogService.Repository;

public class MySqlLogMessageRepository(MySqlDbContext context) : ILogMessageRepository
{
	public IAsyncEnumerable<LogMessageModel> GetAll(Guid userId)
	{
		return context.LogMessages
			.Where(m => m.CreatedBy == userId)
			.AsNoTracking()
			.AsAsyncEnumerable();
	}

	public IQueryable<LogMessageModel> GetAllQueryable()
	{
		return context.LogMessages
			.AsNoTracking()
			.AsQueryable();
	}

	public async Task<LogMessageModel?> Get(Guid id)
	{
		return await context.LogMessages.FirstOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);
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

		context.AddRange(models);

		await context.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task Delete(Guid[] ids)
	{
		await context.LogMessages.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync().ConfigureAwait(false);
		await context.SaveChangesAsync().ConfigureAwait(false);
	}
}