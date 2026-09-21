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
            string.IsNullOrWhiteSpace(Input.ImageUrl) ? null : Input.ImageUrl);

        var ok = await api.CreateExternalExperienceAsync(User.GetJwt()!, User.GetHotelId(), request);
        this.SetToast(ok, "Experiência criada com sucesso!", "Não foi possível criar a experiência.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id, string name, string category, string description, decimal price, string durationLabel, string location, string? imageUrl, bool isActive)
    {
        var request = new UpdateExternalExperienceRequest(name, description, category, price, durationLabel, location, string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl, isActive);
        var ok = await api.UpdateExternalExperienceAsync(User.GetJwt()!, User.GetHotelId(), id, request);
        this.SetToast(ok, "Experiência atualizada com sucesso!", "Não foi possível salvar as alterações.");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var ok = await api.DeleteExternalExperienceAsync(User.GetJwt()!, User.GetHotelId(), id);
        this.SetToast(ok, "Experiência excluída.", "Não foi possível excluir a experiência.");
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
        public string? ImageUrl { get; set; }
    }
}
