using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class ServicosModel(ApiClient api) : PageModel
{
    public List<ServiceDto> Services { get; set; } = [];
    public List<ServiceRequestDto> Requests { get; set; } = [];

    [BindProperty]
    public CreateServiceInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Services = await api.GetServicesAsync(User.GetHotelId());
        Requests = await api.GetServiceRequestsAsync(User.GetJwt()!, User.GetHotelId());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Services = await api.GetServicesAsync(User.GetHotelId());
            Requests = await api.GetServiceRequestsAsync(User.GetJwt()!, User.GetHotelId());
            return Page();
        }

        var request = new CreateServiceItemRequest(
            Input.Name,
            Input.Description,
            Input.Category,
            Input.Price,
            Input.DurationMinutes,
            null);

        await api.CreateServiceAsync(User.GetJwt()!, User.GetHotelId(), request);

        return RedirectToPage();
    }

    public class CreateServiceInput
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; } = 60;
    }
}
