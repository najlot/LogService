using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogService.Client.Data.Services;
using LogService.Client.Data.Models;

namespace LogService.Razor.Pages;

[Authorize]
public class LogMessageModel : PageModel
{
    private readonly ILogMessageService _logMessageService;
    private readonly ILogger<LogMessageModel> _logger;

    public LogMessageModel(ILogMessageService logMessageService, ILogger<LogMessageModel> logger)
    {
        _logMessageService = logMessageService;
        _logger = logger;
    }

    public LogMessageDetailModel? LogMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        try
        {
            LogMessage = await _logMessageService.GetAsync(id);

            if (LogMessage == null)
            {
                ErrorMessage = "Log message not found.";
                return Page();
            }

            _logger.LogDebug("Loaded log message {Id}", id);
            return Page();
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            // Redirect to login if not authenticated
            return Redirect("/Account/Login?returnUrl=" + Uri.EscapeDataString(Request.Path));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading log message {Id}", id);
            ErrorMessage = "Error loading log message. Please try again.";
            return Page();
        }
    }
}