using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.Institutional.Pages;

public class ContactModel(IHttpClientFactory httpClientFactory, ILogger<ContactModel> logger) : PageModel
{
    [BindProperty]
    public LeadInput Input { get; set; } = new();

    public bool Submitted { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var client = httpClientFactory.CreateClient("AllStayApi");

        try
        {
            var response = await client.PostAsJsonAsync("/api/leads", new
            {
                hotelName = Input.HotelName,
                contactName = Input.ContactName,
                email = Input.Email,
                phone = Input.Phone,
                message = Input.Message
            });

            if (response.IsSuccessStatusCode)
            {
                Submitted = true;
                ModelState.Clear();
                Input = new LeadInput();
            }
            else
            {
                ErrorMessage = "Não foi possível enviar sua mensagem agora. Tente novamente em instantes.";
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to submit lead to AllStay API.");
            ErrorMessage = "Não foi possível enviar sua mensagem agora. Tente novamente em instantes.";
        }

        return Page();
    }

    public class LeadInput
    {
        [Required(ErrorMessage = "Informe o nome do hotel.")]
        [Display(Name = "Nome do hotel")]
        public string HotelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe seu nome.")]
        [Display(Name = "Seu nome")]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe um e-mail válido.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefone")]
        public string? Phone { get; set; }

        [Display(Name = "Mensagem")]
        public string? Message { get; set; }
    }
}
