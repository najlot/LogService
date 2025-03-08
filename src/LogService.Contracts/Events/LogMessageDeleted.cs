using System;

namespace LogService.Contracts.Events;

public class LogMessageDeleted(Guid id)
{
	public Guid Id { get; } = id;
}