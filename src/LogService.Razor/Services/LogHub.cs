using Microsoft.AspNetCore.SignalR;
using LogService.Contracts.Events;
using Cosei.Client.Base;
using Cosei.Client.Http;
using LogService.Client.Data.Identity;

namespace LogService.Razor.Services;

public class LogHub : Hub
{
	private readonly ILogger<LogHub> _logger;
	private readonly ITokenProvider _tokenProvider;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly Dictionary<string, ISubscriber> _subscribers = new();

	public LogHub(ILogger<LogHub> logger, ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory)
	{
		_logger = logger;
		_tokenProvider = tokenProvider;
		_httpClientFactory = httpClientFactory;
	}

	public async Task JoinUserGroup(string userId)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
		_logger.LogDebug("User {UserId} joined group User_{UserId}", userId, userId);

		// Create subscriber for this user if not exists
		if (!_subscribers.ContainsKey(userId))
		{
			try
			{
				using var client = _httpClientFactory.CreateClient();
				var serverUri = (client?.BaseAddress) ?? throw new NullReferenceException("Could not retrieve server connection information!");
				var token = await _tokenProvider.GetToken();
				var signalRUri = new Uri(serverUri, "/cosei");

				var subscriber = new SignalRSubscriber(signalRUri.AbsoluteUri,
					options =>
					{
						options.Headers.Add("Authorization", $"Bearer {token}");
					},
					exception =>
					{
						_logger.LogError(exception, "Subscriber error for user {UserId}", userId);
					});

				// Register event handlers - only for events that exist
				subscriber.Register<List<LogMessageCreated>>(async (logMessage) => await HandleLogMessagesCreated(userId, logMessage));

				await subscriber.StartAsync();
				_subscribers[userId] = subscriber;

				_logger.LogDebug("SignalR subscriber created and started for user {UserId}", userId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create subscriber for user {UserId}", userId);
			}
		}
	}

	public async Task LeaveUserGroup(string userId)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
		_logger.LogDebug("User {UserId} left group User_{UserId}", userId, userId);
	}

	private async Task HandleLogMessagesCreated(string userId, List<LogMessageCreated> logMessages)
	{
		await Clients.Group($"User_{userId}").SendAsync("ReceiveLogMessages", logMessages);
		_logger.LogDebug("Sent log message created event to user group User_{userId}", userId);
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_logger.LogDebug("Client {ConnectionId} disconnected", Context.ConnectionId);
		await base.OnDisconnectedAsync(exception);
	}

	public override async Task OnConnectedAsync()
	{
		_logger.LogDebug("Client {ConnectionId} connected", Context.ConnectionId);
		await base.OnConnectedAsync();
	}
}