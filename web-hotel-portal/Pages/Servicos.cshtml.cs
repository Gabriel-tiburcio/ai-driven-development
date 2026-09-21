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
            string.IsNullOrWhiteSpace(Input.ImageUrl) ? null : Input.ImageUrl);

        var ok = await api.CreateServiceAsync(User.GetJwt()!, User.GetHotelId(), request);
        this.SetToast(ok, "Serviço criado com sucesso!", "Não foi possível criar o serviço.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id, string name, string description, string category, decimal price, int durationMinutes, string? imageUrl, bool isActive)
    {
        var request = new UpdateServiceItemRequest(name, description, category, price, durationMinutes, string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl, isActive);
        var ok = await api.UpdateServiceAsync(User.GetJwt()!, User.GetHotelId(), id, request);
        this.SetToast(ok, "Serviço atualizado com sucesso!", "Não foi possível salvar as alterações.");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var ok = await api.DeleteServiceAsync(User.GetJwt()!, User.GetHotelId(), id);
        this.SetToast(ok, "Serviço excluído.", "Não foi possível excluir o serviço.");
        return RedirectToPage();
    }

    public class CreateServiceInput
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; } = 60;
        public string? ImageUrl { get; set; }
    }
}
