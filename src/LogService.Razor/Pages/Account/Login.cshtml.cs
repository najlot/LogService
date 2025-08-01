using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LogService.Client.Data.Identity;

namespace LogService.Razor.Pages.Account;

public class LoginModel : PageModel
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ITokenService tokenService, ILogger<LoginModel> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet()
    {
        // Clear any existing authentication
        if (User.Identity?.IsAuthenticated == true)
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var token = await _tokenService.CreateToken(Input.Username, Input.Password);

            if (string.IsNullOrWhiteSpace(token))
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            // Create claims for the user
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, Input.Username),
                new("access_token", token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Remember user across browser sessions
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            _logger.LogInformation("User {Username} logged in successfully", Input.Username);

            // Redirect to return URL or home page
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", Input.Username);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }
}