namespace LogService.Contracts.Commands;

public class UpdateUserSettings(int logRetentionDays)
{
	public int LogRetentionDays { get; } = logRetentionDays;
}