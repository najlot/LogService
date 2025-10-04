using System;

namespace LogService.Events;

public class UserDeleted(Guid id)
{
	public Guid Id { get; } = id;
}