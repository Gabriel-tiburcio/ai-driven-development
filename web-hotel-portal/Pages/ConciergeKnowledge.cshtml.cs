using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class ConciergeKnowledgeModel(ApiClient api) : PageModel
{
    public List<ConciergeKnowledgeEntryDto> Entries { get; set; } = [];

    [BindProperty]
    public CreateEntryInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Entries = await api.GetConciergeKnowledgeAsync(User.GetJwt()!, User.GetHotelId());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Entries = await api.GetConciergeKnowledgeAsync(User.GetJwt()!, User.GetHotelId());
            return Page();
        }

        await api.CreateConciergeKnowledgeAsync(
            User.GetJwt()!,
            User.GetHotelId(),
            new CreateConciergeKnowledgeEntryRequest(Input.Title, Input.Content));

        return RedirectToPage();
    }

    public class CreateEntryInput
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
