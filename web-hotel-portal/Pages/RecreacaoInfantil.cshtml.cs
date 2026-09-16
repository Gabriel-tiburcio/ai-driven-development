using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class RecreacaoInfantilModel(ApiClient api) : PageModel
{
    public List<KidsActivityDto> Activities { get; set; } = [];
    public List<KidsEnrollmentDto> Enrollments { get; set; } = [];

    [BindProperty]
    public CreateKidsActivityInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Activities = await api.GetKidsActivitiesAsync(User.GetHotelId());
        Enrollments = await api.GetKidsEnrollmentsAsync(User.GetJwt()!, User.GetHotelId());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Activities = await api.GetKidsActivitiesAsync(User.GetHotelId());
            Enrollments = await api.GetKidsEnrollmentsAsync(User.GetJwt()!, User.GetHotelId());
            return Page();
        }

        var request = new CreateKidsActivityRequest(
            Input.Name,
            string.IsNullOrWhiteSpace(Input.Description) ? null : Input.Description,
            Input.AgeRange,
            Input.Schedule,
            Input.Location,
            null);

        await api.CreateKidsActivityAsync(User.GetJwt()!, User.GetHotelId(), request);

        return RedirectToPage();
    }

    public class CreateKidsActivityInput
    {
        public string Name { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
