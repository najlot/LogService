using System;

namespace LogService.ClientBase.Messages;

public class DeleteLogArgument
{
	public Guid ParentId { get; }
	public int Id { get; }

	public DeleteLogArgument(Guid parentId, int id)
	{
		ParentId = parentId;
		Id = id;
	}
}