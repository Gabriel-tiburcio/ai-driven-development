using System.ComponentModel.DataAnnotations;
using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages.Admin;

[Authorize(AuthenticationSchemes = AuthSchemes.Admin)]
public class IndexModel(ApiClient api) : PageModel
{
    public List<HotelDto> Hotels { get; set; } = [];

    [BindProperty]
    public CreateHotelInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Hotels = await api.GetHotelsAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            Hotels = await api.GetHotelsAsync();
            return Page();
        }

        await api.CreateHotelAsync(new CreateHotelRequest(
            Input.Name,
            Input.Code,
            Input.Tier,
            string.IsNullOrWhiteSpace(Input.ContactEmail) ? null : Input.ContactEmail,
            string.IsNullOrWhiteSpace(Input.ContactPhone) ? null : Input.ContactPhone,
            string.IsNullOrWhiteSpace(Input.Address) ? null : Input.Address,
            string.IsNullOrWhiteSpace(Input.City) ? null : Input.City));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(Guid hotelId, bool activate)
    {
        await api.SetHotelActiveAsync(hotelId, activate);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateLocationAsync(Guid hotelId, string? address, string? city)
    {
        await api.UpdateHotelLocationAsync(hotelId, new UpdateHotelLocationRequest(
            string.IsNullOrWhiteSpace(address) ? null : address,
            string.IsNullOrWhiteSpace(city) ? null : city));
        return RedirectToPage();
    }

    public class CreateHotelInput
    {
        [Required(ErrorMessage = "Informe o nome.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o código.")]
        public string Code { get; set; } = string.Empty;

        public HotelTier Tier { get; set; } = HotelTier.Essential;

        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? ContactEmail { get; set; }

        public string? ContactPhone { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }
    }
}
