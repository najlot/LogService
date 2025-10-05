namespace LogService.Contracts.Commands;

public class CreateLogMessage
{
	public DateTime DateTime { get; set; }
	public int LogLevel { get; set; }
	public string? Category { get; set; }
	public string? State { get; set; }
	public string? RawMessage { get; set; }
	public string? Message { get; set; }
	public string? Exception { get; set; }
	public bool ExceptionIsValid { get; set; }
	public KeyValuePair<string, string>[]? Arguments { get; set; }
}