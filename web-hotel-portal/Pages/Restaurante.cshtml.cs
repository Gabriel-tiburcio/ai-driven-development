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
            string.IsNullOrWhiteSpace(Input.ImageUrl) ? null : Input.ImageUrl,
            menuHighlights);

        var ok = await api.CreateRestaurantAsync(User.GetJwt()!, User.GetHotelId(), request);
        this.SetToast(ok, "Restaurante criado com sucesso!", "Não foi possível criar o restaurante.");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id, string name, string description, string cuisineType, string hours, string? menuHighlights, string? imageUrl, bool isActive)
    {
        var highlights = (menuHighlights ?? string.Empty)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var request = new UpdateRestaurantRequest(name, description, cuisineType, hours, string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl, highlights, isActive);
        var ok = await api.UpdateRestaurantAsync(User.GetJwt()!, User.GetHotelId(), id, request);
        this.SetToast(ok, "Restaurante atualizado com sucesso!", "Não foi possível salvar as alterações.");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var ok = await api.DeleteRestaurantAsync(User.GetJwt()!, User.GetHotelId(), id);
        this.SetToast(ok, "Restaurante excluído.", "Não foi possível excluir o restaurante.");
        return RedirectToPage();
    }

    public class CreateRestaurantInput
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CuisineType { get; set; } = string.Empty;
        public string Hours { get; set; } = string.Empty;
        public string? MenuHighlights { get; set; }
        public string? ImageUrl { get; set; }
    }
}
