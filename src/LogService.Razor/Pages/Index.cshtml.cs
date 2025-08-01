using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogService.Client.Data.Services;
using LogService.Client.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace LogService.Razor.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public TokenInputModel TokenInput { get; set; } = new();

    [BindProperty]
    public SettingsInputModel SettingsInput { get; set; } = new();

    public string? ServiceToken { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public class TokenInputModel
    {
        public string Source { get; set; } = string.Empty;
        public DateTime ValidUntil { get; set; } = DateTime.Now.AddYears(1);
    }

    public class SettingsInputModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "Log retention days must be a positive number")]
        public int LogRetentionDays { get; set; }
    }

    public async Task OnGetAsync()
    {
        try
        {
            var userModel = await _userService.GetCurrentUserAsync();
            SettingsInput.LogRetentionDays = userModel.Settings.LogRetentionDays;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user settings");
            ErrorMessage = "Error loading user settings";
        }
    }

    public async Task<IActionResult> OnPostGenerateTokenAsync()
    {
        try
        {
            // TODO: Implement service token generation
            // This would call the LogService API to generate a service token
            ServiceToken = "Service token generation not yet implemented for Razor pages";
            _logger.LogWarning("Service token generation requested but not implemented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating service token");
            ErrorMessage = "Error generating service token";
        }

        // Reload user settings
        await OnGetAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostSaveSettingsAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        try
        {
            if (SettingsInput.LogRetentionDays < 0)
            {
                SettingsInput.LogRetentionDays *= -1;
            }

            await _userService.UpdateSettingsAsync(new UserSettingsModel
            {
                LogRetentionDays = SettingsInput.LogRetentionDays
            });

            SuccessMessage = "Settings saved successfully";
            _logger.LogInformation("User settings updated for {Username}", User.Identity?.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user settings");
            ErrorMessage = "Error updating settings";
        }

        await OnGetAsync();
        return Page();
    }
}