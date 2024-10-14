using LogService.Client.Data.Models;
using LogService.Contracts;

namespace LogService.Client.Data.Mappings
{
	public sealed class LogArgumentMapper
	{
		public LogArgument Map(LogArgumentModel src, LogArgument dest)
		{
			dest.Id = src.Id;
			dest.Key = src.Key;
			dest.Value = src.Value;

			return dest;
		}

		public LogArgumentModel Map(LogArgument src, LogArgumentModel dest)
		{
			dest.Id = src.Id;
			dest.Key = src.Key;
			dest.Value = src.Value;

			return dest;
		}
	}
}
