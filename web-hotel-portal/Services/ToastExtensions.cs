using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllStay.Web.HotelPortal.Services;

/// <summary>Stashes a one-shot success/error message in TempData for _Layout.cshtml to show via SweetAlert2 after the redirect.</summary>
public static class ToastExtensions
{
    public static void SetToast(this PageModel page, bool success, string successMessage, string errorMessage)
    {
        page.TempData["ToastType"] = success ? "success" : "error";
        page.TempData["ToastMessage"] = success ? successMessage : errorMessage;
    }
}
