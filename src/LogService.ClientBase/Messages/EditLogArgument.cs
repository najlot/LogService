using System;

namespace LogService.ClientBase.Messages;

public class EditLogArgument
{
	public Guid ParentId { get; }
	public int Id { get; }

	public EditLogArgument(Guid parentId, int id)
	{
		ParentId = parentId;
		Id = id;
	}
}