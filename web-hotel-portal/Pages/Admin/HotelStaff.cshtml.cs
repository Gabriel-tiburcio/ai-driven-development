using System.ComponentModel.DataAnnotations;
using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages.Admin;

[Authorize(AuthenticationSchemes = AuthSchemes.Admin)]
public class HotelStaffModel(ApiClient api) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid HotelId { get; set; }

    public HotelDto? Hotel { get; set; }
    public List<HotelStaffDto> Staff { get; set; } = [];

    [BindProperty]
    public CreateStaffInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Hotel = (await api.GetHotelsAsync()).FirstOrDefault(h => h.Id == HotelId);
        if (Hotel is null)
            return NotFound();

        Staff = await api.GetHotelStaffAsync(HotelId);
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        Hotel = (await api.GetHotelsAsync()).FirstOrDefault(h => h.Id == HotelId);
        if (Hotel is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            Staff = await api.GetHotelStaffAsync(HotelId);
            return Page();
        }

        await api.CreateStaffAsync(HotelId, new CreateStaffRequest(Input.Email, Input.Password, Input.FullName, Input.Role));

        return RedirectToPage(new { hotelId = HotelId });
    }

    public async Task<IActionResult> OnPostToggleAsync(Guid staffId, bool activate)
    {
        await api.SetStaffActiveAsync(HotelId, staffId, activate);
        return RedirectToPage(new { hotelId = HotelId });
    }

    public class CreateStaffInput
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o nome.")]
        public string FullName { get; set; } = string.Empty;

        public StaffRole Role { get; set; } = StaffRole.FrontDesk;
    }
}
