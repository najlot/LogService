using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using LogService.Client.Data.Identity;

namespace LogService.Razor.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IRegistrationService registrationService, ILogger<RegisterModel> logger)
    {
        _registrationService = registrationService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task OnGetAsync()
    {
        // Log out any existing user before showing registration form
        if (User.Identity?.IsAuthenticated == true)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await _registrationService.Register(Guid.NewGuid(), Input.Username, Input.Email, Input.Password);

            if (result.IsSuccess)
            {
                SuccessMessage = "Registration successful! You can now login with your credentials.";
                _logger.LogInformation("User {Username} registered successfully", Input.Username);
                
                // Clear the form
                Input = new InputModel();
                return Page();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Registration failed. Username may already exist.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Username}", Input.Username);
            ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }
}