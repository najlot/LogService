using System;

namespace LogService.Contracts.Events;

public class UserDeleted(Guid id)
{
	public Guid Id { get; } = id;
}