using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogService.Service.Model;

namespace LogService.Service.Repository;

public interface ILogMessageRepository
{
	IAsyncEnumerable<LogMessageModel> GetAll(Guid userId);

	IQueryable<LogMessageModel> GetAllQueryable();

	Task<LogMessageModel?> Get(Guid id);

	Task Insert(LogMessageModel model);
	Task Insert(LogMessageModel[] models);

	Task Delete(Guid id);
}