using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
        => RedirectToPage(User.Identity?.IsAuthenticated == true ? "/Activities" : "/Login");
}
