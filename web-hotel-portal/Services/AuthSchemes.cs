namespace AllStay.Web.HotelPortal.Services;

/// <summary>Two independent cookie sessions: hotel staff (per-hotel, JWT-backed) and platform admin (shared operator key).</summary>
public static class AuthSchemes
{
    public const string Staff = "Staff";
    public const string Admin = "Admin";
}
