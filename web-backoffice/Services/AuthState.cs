using AllStay.Web.Backoffice.Models;

namespace AllStay.Web.Backoffice.Services;

/// <summary>
/// Holds the current staff session in memory for the lifetime of the Blazor Server circuit.
/// MVP-scope: no persistence across page reloads (staff simply logs in again).
/// </summary>
public class AuthState
{
    public string? Token { get; private set; }
    public StaffProfileDto? Staff { get; private set; }

    public bool IsAuthenticated => Token is not null && Staff is not null;

    public event Action? OnChange;

    public void SetSession(LoginResponse login)
    {
        Token = login.Token;
        Staff = login.Staff;
        OnChange?.Invoke();
    }

    public void Clear()
    {
        Token = null;
        Staff = null;
        OnChange?.Invoke();
    }
}
