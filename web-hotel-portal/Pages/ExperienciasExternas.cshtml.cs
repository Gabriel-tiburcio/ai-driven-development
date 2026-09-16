using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class ExperienciasExternasModel(ApiClient api) : PageModel
{
    public List<ExternalExperienceDto> Experiences { get; set; } = [];
    public List<ExperienceRequestDto> Requests { get; set; } = [];

    [BindProperty]
    public CreateExperienceInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Experiences = await api.GetExternalExperiencesAsync(User.GetHotelId());
        Requests = await api.GetExperienceRequestsAsync(User.GetJwt()!, User.GetHotelId());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Experiences = await api.GetExternalExperiencesAsync(User.GetHotelId());
            Requests = await api.GetExperienceRequestsAsync(User.GetJwt()!, User.GetHotelId());
            return Page();
        }

        var request = new CreateExternalExperienceRequest(
            Input.Name,
            Input.Description,
            Input.Category,
            Input.Price,
            Input.DurationLabel,
            Input.Location,
            null);

        await api.CreateExternalExperienceAsync(User.GetJwt()!, User.GetHotelId(), request);

        return RedirectToPage();
    }

    public class CreateExperienceInput
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string DurationLabel { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
