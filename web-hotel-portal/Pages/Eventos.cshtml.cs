using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class EventosModel(ApiClient api) : PageModel
{
    public List<EventDto> Events { get; set; } = [];

    [BindProperty]
    public CreateEventInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Events = await api.GetEventsAsync(User.GetHotelId());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Events = await api.GetEventsAsync(User.GetHotelId());
            return Page();
        }

        var request = new CreateEventRequest(
            Input.Name,
            string.IsNullOrWhiteSpace(Input.Description) ? null : Input.Description,
            Input.Category,
            Input.EventDate,
            Input.StartTime,
            Input.Location,
            string.IsNullOrWhiteSpace(Input.ImageUrl) ? null : Input.ImageUrl);

        var ok = await api.CreateEventAsync(User.GetJwt()!, User.GetHotelId(), request);
        this.SetToast(ok, "Evento criado com sucesso!", "Não foi possível criar o evento.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id, string name, string category, string? description, DateOnly eventDate, TimeOnly startTime, string location, string? imageUrl, bool isActive)
    {
        var request = new UpdateEventRequest(
            name,
            string.IsNullOrWhiteSpace(description) ? null : description,
            category,
            eventDate,
            startTime,
            location,
            string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
            isActive);

        var ok = await api.UpdateEventAsync(User.GetJwt()!, User.GetHotelId(), id, request);
        this.SetToast(ok, "Evento atualizado com sucesso!", "Não foi possível salvar as alterações.");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var ok = await api.DeleteEventAsync(User.GetJwt()!, User.GetHotelId(), id);
        this.SetToast(ok, "Evento excluído.", "Não foi possível excluir o evento.");
        return RedirectToPage();
    }

    public class CreateEventInput
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public TimeOnly StartTime { get; set; } = new(19, 0);
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
