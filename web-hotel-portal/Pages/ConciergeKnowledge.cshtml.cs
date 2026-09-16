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

        var ok = await api.CreateConciergeKnowledgeAsync(
            User.GetJwt()!,
            User.GetHotelId(),
            new CreateConciergeKnowledgeEntryRequest(Input.Title, Input.Content));
        this.SetToast(ok, "Nota criada com sucesso!", "Não foi possível criar a nota.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id, string title, string content)
    {
        var ok = await api.UpdateConciergeKnowledgeAsync(User.GetJwt()!, User.GetHotelId(), id, new UpdateConciergeKnowledgeEntryRequest(title, content));
        this.SetToast(ok, "Nota atualizada com sucesso!", "Não foi possível salvar as alterações.");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var ok = await api.DeleteConciergeKnowledgeAsync(User.GetJwt()!, User.GetHotelId(), id);
        this.SetToast(ok, "Nota excluída.", "Não foi possível excluir a nota.");
        return RedirectToPage();
    }

    public class CreateEntryInput
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
