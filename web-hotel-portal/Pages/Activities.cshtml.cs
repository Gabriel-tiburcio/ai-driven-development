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
            string.IsNullOrWhiteSpace(Input.ImageUrl) ? null : Input.ImageUrl);

        var ok = await api.CreateActivityAsync(User.GetJwt()!, User.GetHotelId(), request);
        this.SetToast(ok, "Atividade criada com sucesso!", "Não foi possível criar a atividade.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id, string name, string category, string? description, decimal price, int durationMinutes, string? imageUrl, bool isActive)
    {
        var request = new UpdateActivityRequest(
            name,
            string.IsNullOrWhiteSpace(description) ? null : description,
            category,
            price,
            durationMinutes,
            string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
            isActive);

        var ok = await api.UpdateActivityAsync(User.GetJwt()!, User.GetHotelId(), id, request);
        this.SetToast(ok, "Atividade atualizada com sucesso!", "Não foi possível salvar as alterações.");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var ok = await api.DeleteActivityAsync(User.GetJwt()!, User.GetHotelId(), id);
        this.SetToast(ok, "Atividade excluída.", "Não foi possível excluir a atividade.");
        return RedirectToPage();
    }

    public class CreateActivityInput
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; } = 60;
        public string? ImageUrl { get; set; }
    }
}
