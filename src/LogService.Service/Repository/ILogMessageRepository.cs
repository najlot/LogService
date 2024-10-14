using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Service.Model;

namespace LogService.Service.Repository
{
	public interface ILogMessageRepository
	{
		IAsyncEnumerable<LogMessageModel> GetAll();

		Task<LogMessageModel> Get(Guid id);

		Task Insert(LogMessageModel model);

		Task Update(LogMessageModel model);

		Task Delete(Guid id);
	}
}