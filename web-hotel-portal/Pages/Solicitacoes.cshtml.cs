using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class SolicitacoesModel(ApiClient api) : PageModel
{
    public List<GuestRequestDto> Requests { get; set; } = [];

    public async Task OnGetAsync()
    {
        Requests = await api.GetGuestRequestsAsync(User.GetJwt()!, User.GetHotelId());
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(Guid requestId, GuestRequestStatus status)
    {
        await api.UpdateGuestRequestStatusAsync(User.GetJwt()!, User.GetHotelId(), requestId, status);
        return RedirectToPage();
    }
}
