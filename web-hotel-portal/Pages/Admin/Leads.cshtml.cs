using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages.Admin;

[Authorize(AuthenticationSchemes = AuthSchemes.Admin)]
public class LeadsModel(ApiClient api) : PageModel
{
    public List<LeadDto> Leads { get; set; } = [];

    public async Task OnGetAsync()
    {
        Leads = await api.GetLeadsAsync();
    }
}
