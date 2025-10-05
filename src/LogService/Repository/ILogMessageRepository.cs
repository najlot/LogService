using LogService.Model;

namespace LogService.Repository;

public interface ILogMessageRepository
{
	IAsyncEnumerable<LogMessageModel> GetAll(Guid userId);

	IQueryable<LogMessageModel> GetAllQueryable();

	Task<LogMessageModel?> Get(Guid id);

	Task Insert(LogMessageModel[] models);

	Task Delete(Guid[] ids);
}