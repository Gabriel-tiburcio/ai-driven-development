using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages.Admin;

public class LoginModel(IConfiguration configuration) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await HttpContext.AuthenticateAsync(AuthSchemes.Admin);
        if (result.Succeeded)
            return RedirectToPage("/Admin/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var expectedKey = configuration["Admin:ApiKey"];
        if (string.IsNullOrEmpty(expectedKey) || Input.Password != expectedKey)
        {
            ErrorMessage = "Senha inválida.";
            return Page();
        }

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "Administrador")], AuthSchemes.Admin);
        await HttpContext.SignInAsync(AuthSchemes.Admin, new ClaimsPrincipal(identity));

        return RedirectToPage("/Admin/Index");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Informe a senha de administrador.")]
        public string Password { get; set; } = string.Empty;
    }
}
