using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class ReservationsModel(ApiClient api) : PageModel
{
    public List<ReservationDto> Reservations { get; set; } = [];

    public async Task OnGetAsync()
    {
        Reservations = await api.GetReservationsAsync(User.GetJwt()!, User.GetHotelId());
    }
}
