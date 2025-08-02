using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogService.Client.Data.Services;
using LogService.Client.Data.Models;
using LogService.Contracts.Filters;

namespace LogService.Razor.Pages;

[Authorize]
public class LogMessagesModel : PageModel
{
    private readonly ILogMessageService _logMessageService;
    private readonly ILogger<LogMessagesModel> _logger;

    public LogMessagesModel(ILogMessageService logMessageService, ILogger<LogMessagesModel> logger)
    {
        _logMessageService = logMessageService;
        _logger = logger;
    }

    public LogMessageListItemModel[] LogMessages { get; set; } = Array.Empty<LogMessageListItemModel>();
    public LogMessageFilter Filter { get; set; } = new();
    public bool IsLoading { get; set; } = true;
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(
        DateTime? dateTimeFrom = null,
        DateTime? dateTimeTo = null,
        LogService.Contracts.LogLevel? logLevel = null,
        string? source = null,
        string? category = null,
        string? message = null,
        string? exception = null,
        string? rawMessage = null)
    {
        try
        {
            IsLoading = true;

            // Build filter from query parameters
            Filter = new LogMessageFilter
            {
                DateTimeFrom = dateTimeFrom,
                DateTimeTo = dateTimeTo,
                LogLevel = logLevel ?? LogService.Contracts.LogLevel.Trace,
                Source = source,
                Category = category,
                Message = message,
                Exception = exception,
                RawMessage = rawMessage
            };

            // Load log messages with filter
            LogMessages = await _logMessageService.GetItemsAsync(Filter);

            _logger.LogDebug("Loaded {Count} log messages with filter", LogMessages.Length);
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            // Redirect to login if not authenticated
            Response.Redirect("/Account/Login?returnUrl=" + Uri.EscapeDataString(Request.Path + Request.QueryString));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading log messages");
            ErrorMessage = "Error loading log messages. Please try again.";
            LogMessages = Array.Empty<LogMessageListItemModel>();
        }
        finally
        {
            IsLoading = false;
        }
    }
}