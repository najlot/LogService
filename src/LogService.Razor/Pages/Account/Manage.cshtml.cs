using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using LogService.Client.Data.Services;

namespace LogService.Razor.Pages.Account;

[Authorize]
public class ManageModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<ManageModel> _logger;

    public ManageModel(IUserService userService, ILogger<ManageModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters.")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet()
    {
        // Page loads with empty form
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var user = await _userService.GetCurrentUserAsync();
			user.Password = Input.NewPassword;
			await _userService.UpdateItemAsync(user);
			SuccessMessage = "Your password has been changed successfully.";
			_logger.LogInformation("User {Username} changed their password successfully.", User.Identity?.Name);
			return Page();
		}
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {Username}", User.Identity?.Name);
            ErrorMessage = "An error occurred while changing your password. Please try again.";
            return Page();
        }
    }
}