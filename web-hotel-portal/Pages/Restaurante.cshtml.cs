using AllStay.Web.HotelPortal.Models;
using AllStay.Web.HotelPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Pages;

[Authorize]
public class RestauranteModel(ApiClient api) : PageModel
{
    public List<RestaurantDto> Restaurants { get; set; } = [];

    [BindProperty]
    public CreateRestaurantInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Restaurants = await api.GetRestaurantsAsync(User.GetHotelId());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Restaurants = await api.GetRestaurantsAsync(User.GetHotelId());
            return Page();
        }

        var menuHighlights = (Input.MenuHighlights ?? string.Empty)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var request = new CreateRestaurantRequest(
            Input.Name,
            Input.Description,
            Input.CuisineType,
            Input.Hours,
            null,
            menuHighlights);

        await api.CreateRestaurantAsync(User.GetJwt()!, User.GetHotelId(), request);

        return RedirectToPage();
    }

    public class CreateRestaurantInput
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CuisineType { get; set; } = string.Empty;
        public string Hours { get; set; } = string.Empty;
        public string? MenuHighlights { get; set; }
    }
}
