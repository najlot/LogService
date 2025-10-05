namespace LogService.Model;

public class LogMessageListItemModel
{
	public Guid Id { get; set; }
	public DateTime DateTime { get; set; }
	public Contracts.LogLevel LogLevel { get; set; }
	public string Category { get; set; } = string.Empty;
	public string Source { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
	public bool HasException { get; set; }
}