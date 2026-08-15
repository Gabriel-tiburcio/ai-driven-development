using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class ActivityDetailModel(ApiClient api) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public ActivityDto? Activity { get; set; }

    [BindProperty]
    public AddSlotInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Activity = await api.GetActivityAsync(User.GetHotelId(), Id);
        if (Activity is null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAddSlotAsync()
    {
        var startTime = new DateTimeOffset(Input.Date.ToDateTime(Input.Time), TimeSpan.Zero);
        await api.AddSlotAsync(User.GetJwt()!, User.GetHotelId(), Id, new CreateActivitySlotRequest(startTime, Input.Capacity));
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostDeleteSlotAsync(Guid slotId)
    {
        await api.DeleteSlotAsync(User.GetJwt()!, User.GetHotelId(), Id, slotId);
        return RedirectToPage(new { Id });
    }

    public class AddSlotInput
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public TimeOnly Time { get; set; } = new(9, 0);
        public int Capacity { get; set; } = 8;
    }
}
