using Microsoft.AspNetCore.SignalR;
using LogService.Contracts.Events;

namespace LogService.Razor.Services;

public class LogHub : Hub
{
	private readonly ILogger<LogHub> _logger;

	public LogHub(ILogger<LogHub> logger)
	{
		_logger = logger;
	}

	public async Task JoinUserGroup(string userId)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
		_logger.LogDebug("User {UserId} joined group User_{UserId}", userId, userId);
	}

	public async Task LeaveUserGroup(string userId)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
		_logger.LogDebug("User {UserId} left group User_{UserId}", userId, userId);
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_logger.LogDebug("Client {ConnectionId} disconnected", Context.ConnectionId);
		await base.OnDisconnectedAsync(exception);
	}
}