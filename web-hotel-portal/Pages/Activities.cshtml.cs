using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class ActivitiesModel(ApiClient api) : PageModel
{
    public List<ActivityDto> Activities { get; set; } = [];

    [BindProperty]
    public CreateActivityInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Activities = await api.GetActivitiesAsync(User.GetHotelId());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Activities = await api.GetActivitiesAsync(User.GetHotelId());
            return Page();
        }

        var request = new CreateActivityRequest(
            Input.Name,
            string.IsNullOrWhiteSpace(Input.Description) ? null : Input.Description,
            Input.Category,
            Input.Price,
            Input.DurationMinutes,
            null);

        await api.CreateActivityAsync(User.GetJwt()!, User.GetHotelId(), request);

        return RedirectToPage();
    }

    public class CreateActivityInput
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; } = 60;
    }
}
