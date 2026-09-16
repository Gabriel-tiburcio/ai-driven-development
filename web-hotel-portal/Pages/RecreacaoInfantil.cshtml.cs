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

        var ok = await api.CreateKidsActivityAsync(User.GetJwt()!, User.GetHotelId(), request);
        this.SetToast(ok, "Atividade criada com sucesso!", "Não foi possível criar a atividade.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id, string name, string? description, string ageRange, string schedule, string location, bool isActive)
    {
        var request = new UpdateKidsActivityRequest(
            name,
            string.IsNullOrWhiteSpace(description) ? null : description,
            ageRange,
            schedule,
            location,
            null,
            isActive);

        var ok = await api.UpdateKidsActivityAsync(User.GetJwt()!, User.GetHotelId(), id, request);
        this.SetToast(ok, "Atividade atualizada com sucesso!", "Não foi possível salvar as alterações.");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var ok = await api.DeleteKidsActivityAsync(User.GetJwt()!, User.GetHotelId(), id);
        this.SetToast(ok, "Atividade excluída.", "Não foi possível excluir a atividade.");
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
