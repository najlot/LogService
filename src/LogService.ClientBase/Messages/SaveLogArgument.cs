using System;
using LogService.Client.Data.Models;
using LogService.Contracts;

namespace LogService.ClientBase.Messages;

public class SaveLogArgument
{
	public Guid ParentId { get; }
	public LogArgumentModel Item { get; }

	public SaveLogArgument(Guid parentId, LogArgumentModel item)
	{
		ParentId = parentId;
		Item = item;
	}
}