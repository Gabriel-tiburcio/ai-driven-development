using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AllStay.Api.Auth;

/// <summary>
/// Gates hotel-onboarding endpoints with a shared secret (header X-Admin-Key) instead of
/// building a full admin-role system for what is, for now, an operator-only, low-volume task.
/// </summary>
public class AdminApiKeyAttribute : ActionFilterAttribute
{
    private const string HeaderName = "X-Admin-Key";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expectedKey = config["Admin:ApiKey"];

        if (string.IsNullOrEmpty(expectedKey))
        {
            context.Result = new ObjectResult(new { message = "Admin API is not configured on this environment." })
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
            providedKey != expectedKey)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
