using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class InformacoesModel(ApiClient api) : PageModel
{
    public List<HotelInfoSectionDto> Sections { get; set; } = [];

    [BindProperty]
    public CreateInfoSectionInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Sections = await api.GetInfoSectionsAsync(User.GetHotelId());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Sections = await api.GetInfoSectionsAsync(User.GetHotelId());
            return Page();
        }

        var request = new CreateHotelInfoSectionRequest(Input.Title, Input.Icon, Input.Content, Input.SortOrder);
        var ok = await api.CreateInfoSectionAsync(User.GetJwt()!, User.GetHotelId(), request);
        this.SetToast(ok, "Seção criada com sucesso!", "Não foi possível criar a seção.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id, string title, string icon, string content, int sortOrder, bool isActive)
    {
        var request = new UpdateHotelInfoSectionRequest(title, icon, content, sortOrder, isActive);
        var ok = await api.UpdateInfoSectionAsync(User.GetJwt()!, User.GetHotelId(), id, request);
        this.SetToast(ok, "Seção atualizada com sucesso!", "Não foi possível salvar as alterações.");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var ok = await api.DeleteInfoSectionAsync(User.GetJwt()!, User.GetHotelId(), id);
        this.SetToast(ok, "Seção excluída.", "Não foi possível excluir a seção.");
        return RedirectToPage();
    }

    public class CreateInfoSectionInput
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = "ℹ️";
        public string Content { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
    }
}
