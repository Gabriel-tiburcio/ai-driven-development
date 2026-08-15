using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

public class LoginModel(ApiClient api, ILogger<LoginModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Activities");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        Models.LoginResponse? result;
        try
        {
            result = await api.LoginAsync(Input.Email, Input.Password);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to reach AllStay API during login.");
            ErrorMessage = "Não foi possível conectar à API. Tente novamente em instantes.";
            return Page();
        }

        if (result is null)
        {
            ErrorMessage = "E-mail ou senha inválidos.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.Staff.Id.ToString()),
            new(ClaimTypes.Email, result.Staff.Email),
            new(ClaimTypes.Name, result.Staff.FullName),
            new(ClaimTypes.Role, result.Staff.Role.ToString()),
            new(AllStayClaimTypes.HotelId, result.Staff.HotelId.ToString()),
            new(AllStayClaimTypes.HotelName, result.Staff.HotelName),
            new(AllStayClaimTypes.Jwt, result.Token),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToPage("/Activities");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha.")]
        public string Password { get; set; } = string.Empty;
    }
}
